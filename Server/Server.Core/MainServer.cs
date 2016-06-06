using System;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Server.Core
{
    public class MainServer : IMainServer
    {
        private static int _numberOfThreads;
        private readonly ServerProperties _properties;
        private readonly HttpServiceFactory _serviceFactory;
        private readonly IZSocket _socket;

        public MainServer(IZSocket socket, ServerProperties properties, HttpServiceFactory serviceFactory)
        {
            _numberOfThreads = 0;
            AcceptingNewConn = true;
            _socket = socket;
            _properties = properties;
            _serviceFactory = serviceFactory;
        }

        public bool AcceptingNewConn { get; private set; }

        public void Run()
        {
            try
            {
                if (!AcceptingNewConn) return;
                var handler = _socket.Accept();
                new Thread(() => RunningProcess(handler, Guid.NewGuid())).Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void RunningProcess(IZSocket handler, Guid id)
        {
            Interlocked.Increment(ref _numberOfThreads);
            try
            {
                var request = handler.Receive();
                if (request.Length == 0) return;

                var log = request.IndexOf("\r\n", StringComparison.Ordinal) == -1
                    ? request
                    : request.Substring(0, request.IndexOf("\r\n", StringComparison.Ordinal));

                _properties.Io.Print("[" + _properties.Time.GetTime() + "] [<" + id + ">] " + log);

                if (request.Contains("Content-Type: multipart/form-data;"))
                {
                    var contentLength = request
                        .Substring(request.IndexOf("Content-Length: ", StringComparison.Ordinal));
                    var packageSize = int.Parse(contentLength
                        .Substring(16, contentLength.IndexOf("\r\n", StringComparison.Ordinal) - 16));
                    RequestWithMultiPart(id, request, handler, packageSize);
                }
                else
                    NoMultiPart(id, request, handler);
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
            }
        }

        public void StopNewConnAndCleanUp()
        {
            AcceptingNewConn = false;
            _socket.Close();
            while (_numberOfThreads != 0) ;
        }

        private string GetPacketSplit(string request)
        {
            var packetSplit = request.Substring(request.IndexOf("boundary=----"
                , StringComparison.Ordinal) + 13);
            packetSplit = packetSplit.Remove(packetSplit.IndexOf("\r\n"
                , StringComparison.Ordinal));
            return packetSplit;
        }

        private void RequestWithMultiPart(Guid id, string request, IZSocket handler, int packageSize)
        {
            var requestPacket = request;
            var packetSplit = GetPacketSplit(request);
            if (!request.EndsWith($"------{packetSplit}--\r\n"))
                requestPacket += handler.Receive();
            else
                requestPacket += request.Substring(request.IndexOf($"------{packetSplit}"
                    , StringComparison.Ordinal));
            var httpResponce = _properties.DefaultResponse.Clone();
            var processor = _serviceFactory.GetService(request, Assembly.GetExecutingAssembly(), "Server.Core",
                _properties);

            while (!requestPacket.EndsWith($"------{packetSplit}--\r\n"))
            {
                var packet = handler.Receive();
                requestPacket += packet;
            }
            httpResponce = processor.ProcessRequest(requestPacket, httpResponce, _properties);
            SendResponce(handler, httpResponce, id);
        }

        private void NoMultiPart(Guid id, string request, IZSocket handler)
        {
            var processor = _serviceFactory.GetService(request, Assembly.GetExecutingAssembly(), "Server.Core",
                _properties);
            var httpResponce = processor.ProcessRequest(request, _properties.DefaultResponse.Clone(), _properties);
            SendResponce(handler, httpResponce, id);
        }


        private void SendResponce(IZSocket handler, IHttpResponse httpResponce, Guid id)
        {
            handler.Send("HTTP/1.1 " + httpResponce.HttpStatusCode + "\r\n");
            handler.Send("Cache-Control: " + httpResponce.CacheControl + "\r\n");
            handler.Send("Content-Type: " + httpResponce.ContentType + "\r\n");
            if (httpResponce.ContentDisposition != null)
            {
                handler.Send("Content-Disposition: " + httpResponce.ContentDisposition + "; filename = "
                             + httpResponce.Filename +
                             "\r\n");
                handler.Send("Content-Length: " + _properties.FileReader.ReadAllBytes(httpResponce.FilePath).Length +
                             "\r\n\r\n");
                handler.SendFile(httpResponce.FilePath);
            }
            else
            {
                handler.Send("Content-Length: " + Encoding.ASCII.GetBytes(httpResponce.Body).Length +
                             "\r\n\r\n");
                handler.Send(httpResponce.Body);
            }
            _properties.Io.Print("[" + _properties.Time.GetTime() + "] [<" + id + ">] " + httpResponce.HttpStatusCode);
        }
    }
}