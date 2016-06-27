using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Server.Core
{
    public class MainServer : IMainServer
    {
        private static int _numberOfThreads;
        private readonly List<Assembly> _assembly;
        private readonly List<string> _namespaces;
        private readonly ServerProperties _properties;
        private readonly IRequestProcessor _requestProcessor;
        private readonly HttpServiceFactory _serviceFactory;
        private readonly IZSocket _socket;

        public MainServer(IZSocket socket,
            ServerProperties properties,
            HttpServiceFactory serviceFactory,
            IRequestProcessor requestProcessor,
            List<string> namespaces,
            List<Assembly> assembly)
        {
            _numberOfThreads = 0;
            AcceptingNewConn = true;
            _socket = socket;
            _properties = properties;
            _serviceFactory = serviceFactory;
            ThreadPool.SetMaxThreads(100, 100);
            _namespaces = namespaces;
            _assembly = assembly;
            _requestProcessor = requestProcessor;
        }

        public bool AcceptingNewConn { get; private set; }

        public void Run()
        {
            try
            {
                if (!AcceptingNewConn) return;
                var handler = _socket.Accept();
                ThreadPool.QueueUserWorkItem(RunningProcess,
                    new PoolDataForRequest(new HttpResponse(handler), 
                    handler, Guid.NewGuid()));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void RunningProcess(object poolIdAndSocket)
        {
            var handler = ((PoolDataForRequest)poolIdAndSocket).Handler;
            var id = ((PoolDataForRequest)poolIdAndSocket).Id;
            var response = ((PoolDataForRequest) poolIdAndSocket).Response;
            Interlocked.Increment(ref _numberOfThreads);
            var returnCode = "";
            try
            {
                var request = handler.Receive();
                if (request.Length == 0) return;


                var log =
                    request
                        .Substring(0, request.IndexOf("\r\n",
                            StringComparison.Ordinal));

                _properties.Io.Print("[" + _properties.Time.GetTime() + "] [<" + id + ">] " + log);

                var processor = _serviceFactory.GetService(request,
                    _namespaces, _assembly,
                    _properties);

                returnCode
                    = _requestProcessor.HandleRequest(request,
                        handler, processor, _properties,
                        response);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                if (handler.Connected())
                    handler.Close();
                Interlocked.Decrement(ref _numberOfThreads);
                if (returnCode != "")
                    _properties.Io.Print("[" + _properties.Time.GetTime()
                                         + "] [<" + id + ">] "
                                         + returnCode);
            }
        }

        public void StopNewConnAndCleanUp()
        {
            AcceptingNewConn = false;
            _socket.Close();
            while (_numberOfThreads != 0) ;
        }
    }
}