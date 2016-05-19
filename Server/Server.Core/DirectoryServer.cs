using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class DirectoryServer : IMainServer
    {
        IFileProxy fileReader;
        IDataManager socket;
        IWebPageMaker webMaker;
        IDirectoryProxy dirReader;
        string currentDir;
        public DirectoryServer(IDataManager socket, IWebPageMaker webMaker, string currentDir, IDirectoryProxy dirReader, IFileProxy fileReader)
        {
            this.fileReader = fileReader;
            this.dirReader = dirReader;
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
                string requestItem = request.Substring(request.IndexOf("GET /") + 5, request.IndexOf(" HTTP/1.1")-5);
                if (request.Contains("GET / HTTP/1.1"))
                {
                    handler.send("HTTP/1.1 200 OK\r\n");
                    handler.send("Content-Type: text/html\r\n");
                    handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(currentDir, dirReader)).Length + "\r\n\r\n");
                    handler.send(webMaker.directoryContents(currentDir, dirReader));
                }
                else
                {
                    if(fileReader.Exists(requestItem))
                    {
                        pushFile(requestItem, handler);
                    }
                    else if(dirReader.Exists(requestItem))
                    {
                        pushDir(requestItem, handler);
                    }
                    else
                        error404(handler);
                }
            }
            handler.close();
        }
        private void pushDir(string path, IDataManager handler)
        {
            handler.send("HTTP/1.1 200 OK\r\n");
            handler.send("Content-Type: text/html\r\n");
            handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(path, dirReader)).Length + "\r\n\r\n");
            handler.send(webMaker.directoryContents(path, dirReader));
        }
        private void pushFile(string path, IDataManager handler)
        {
            handler.send("HTTP/1.1 200 OK\r\n");
            handler.send("Content-Type: /application/download\r\n");
            handler.send("Content-Length: " + fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.sendFile(path);
        }
        private void error404(IDataManager handler)
        {
            handler.send("HTTP/1.1 404 Not Found\r\n");
            handler.send("Content-Type: text/html\r\n");
            handler.send("Content-Length: " + Encoding.ASCII.GetBytes("404").Length + "\r\n\r\n");
            handler.send("404");
        }

        public bool stillAlive()
        {
            return true;
        }
    }
}
