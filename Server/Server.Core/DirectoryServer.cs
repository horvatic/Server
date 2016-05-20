using System.Text;
using System.Threading;

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
        public void runningProcess(IDataManager handler)
        {
            try
            {
                var request = handler.receive();
                if (!(request.Length == 0))
                {
                    string requestItem = request.Substring(request.IndexOf("GET /") + 5, request.IndexOf(" HTTP/1.1") - 5).Replace("%20", " ");
                    if (request.Contains("GET / HTTP/1.1"))
                    {
                        handler.send("HTTP/1.1 200 OK\r\n");
                        handler.send("Content-Type: text/html\r\n");
                        handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(currentDir, dirReader)).Length + "\r\n\r\n");
                        handler.send(webMaker.directoryContents(currentDir, dirReader));
                    }
                    else
                    {
                        if (fileReader.Exists(requestItem))
                        {
                            pushFile(requestItem, handler);
                        }
                        else if (dirReader.Exists(requestItem))
                        {
                            pushDir(requestItem, handler);
                        }
                        else
                            error404(handler);
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
            new Thread(() => this.runningProcess(handler)).Start();

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
            handler.send("Content-Type: application/octet-stream\r\n");
            handler.send("Content-Disposition: attachment; filename = "+ path + "\r\n");
            handler.send("Content-Length: " + fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.sendFile(path);
        }
        private void error404(IDataManager handler)
        {
            handler.send("HTTP/1.1 404 Not Found\r\n");
            handler.send("Content-Type: text/html\r\n");
            handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.error404Page()).Length + "\r\n\r\n");
            handler.send(webMaker.error404Page());
        }

        public bool stillAlive()
        {
            return true;
        }
    }
}
