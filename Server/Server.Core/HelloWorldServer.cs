using System.Text;
using System.Threading;

namespace Server.Core
{
    public class HelloWorldServer : IMainServer
    {
        IDataManager socket;
        IWebPageMaker webMaker;
        public HelloWorldServer(IDataManager socket, IWebPageMaker webMaker)
        {
            this.socket = socket;
            this.webMaker = webMaker;
        }
        public void runningProcess(IDataManager handler)
        {
            try
            {
                var request = handler.receive();
                if (!(request.Length == 0))
                {
                    if (request.Contains("GET / HTTP/1.1"))
                    {
                        handler.send("HTTP/1.1 200 OK\r\n");
                        handler.send("Content-Type: text/html\r\n");
                        handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.helloWorld()).Length + "\r\n\r\n");
                        handler.send(webMaker.helloWorld());
                    }
                    else
                    {
                        handler.send("HTTP/1.1 404 Not Found\r\n");
                        handler.send("Content-Type: text/html\r\n");
                        handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.error404Page()).Length + "\r\n\r\n");
                        handler.send(webMaker.error404Page());
                    }
                }
            }
            catch (System.Exception e)
            {

            }
            finally
            {
                if(handler.connected())
                    handler.close();
            }
        }
        public void run()
        {
            var handler = socket.accept();
            new Thread(() => runningProcess(handler)).Start();
        }

        public bool stillAlive()
        {
            return true;
        }
    }
}

