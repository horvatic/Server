using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class DirectoryServer : IMainServer
    {
        IDataManager socket;
        IWebPageMaker webMaker;
        string currentDir;
        public DirectoryServer(IDataManager socket, IWebPageMaker webMaker, string currentDir)
        {
            this.socket = socket;
            this.webMaker = webMaker;
            this.currentDir = currentDir;
        }
        public void run()
        {
            var handler = socket.accept();
            var request = handler.receive();
            if (!(request.Length == 0))
            {
                if (request.Contains("GET / HTTP/1.1"))
                {
                    handler.send("HTTP/1.1 200 OK\r\n");
                    handler.send("Content-Type: text/html\r\n");
                    handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(currentDir)).Length + "\r\n\r\n");
                    handler.send(webMaker.directoryContents(currentDir));
                }
                else
                {
                    handler.send("HTTP/1.1 404 Not Found\r\n");
                    handler.send("Content-Type: text/html\r\n");
                    handler.send("Content-Length: " + Encoding.ASCII.GetBytes("404").Length + "\r\n\r\n");
                    handler.send("404");
                }
            }
            handler.close();
        }

        public bool stillAlive()
        {
            return true;
        }
    }
}
