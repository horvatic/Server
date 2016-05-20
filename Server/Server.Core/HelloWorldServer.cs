using System;
using System.Text;
using System.Threading;

namespace Server.Core
{
    public class HelloWorldServer : IMainServer
    {
        private readonly IDataManager _socket;
        private readonly IWebPageMaker _webMaker;

        public HelloWorldServer(IDataManager socket, IWebPageMaker webMaker)
        {
            _socket = socket;
            _webMaker = webMaker;
        }

        public void RunningProcess(IDataManager handler)
        {
            try
            {
                var request = handler.Receive();
                if (request.Length == 0) return;
                if (request.Contains("GET / HTTP/1.1"))
                {
                    handler.Send("HTTP/1.1 200 OK\r\n");
                    handler.Send("Content-Type: text/html\r\n");
                    handler.Send("Content-Length: " + Encoding.ASCII.GetBytes(_webMaker.HelloWorld()).Length +
                                 "\r\n\r\n");
                    handler.Send(_webMaker.HelloWorld());
                }
                else
                {
                    handler.Send("HTTP/1.1 404 Not Found\r\n");
                    handler.Send("Content-Type: text/html\r\n");
                    handler.Send("Content-Length: " + Encoding.ASCII.GetBytes(_webMaker.Error404Page()).Length +
                                 "\r\n\r\n");
                    handler.Send(_webMaker.Error404Page());
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                if (handler.Connected())
                    handler.Close();
            }
        }

        public void Run()
        {
            var handler = _socket.Accept();
            new Thread(() => RunningProcess(handler)).Start();
        }

        public bool StillAlive => true;
    }
}