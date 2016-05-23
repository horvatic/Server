using System;
using System.Text;
using System.Threading;

namespace Server.Core
{
    public class DirectoryServer : IMainServer
    {
        private readonly string _currentDir;
        private readonly IDirectoryProxy _dirReader;
        private readonly IFileProxy _fileReader;
        private readonly IDataManager _socket;
        private readonly IWebPageMaker _webMaker;

        public DirectoryServer(IDataManager socket, IWebPageMaker webMaker, string currentDir, IDirectoryProxy dirReader,
            IFileProxy fileReader)
        {
            _fileReader = fileReader;
            _dirReader = dirReader;
            _socket = socket;
            _webMaker = webMaker;
            _currentDir = currentDir.EndsWith("/") ? currentDir : currentDir + "/";
        }

        public bool StillAlive => true;

        public void RunningProcess(IDataManager handler)
        {
            try
            {
                var request = handler.Receive();
                if (request.Length == 0) return;
                var requestItem =
                    request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                        request.IndexOf(" HTTP/1.1", StringComparison.Ordinal) - 5)
                        .Replace("%20", " ");
                if (request.Contains("GET / HTTP/1.1"))
                {
                    handler.Send("HTTP/1.1 200 OK\r\n");
                    handler.Send("Content-Type: text/html\r\n");
                    handler.Send("Content-Length: " +
                                 Encoding.ASCII.GetBytes(_webMaker.DirectoryContents(_currentDir, _dirReader, _currentDir)).Length +
                                 "\r\n\r\n");
                    handler.Send(_webMaker.DirectoryContents(_currentDir, _dirReader, _currentDir));
                }
                else
                {
                    if (_fileReader.Exists(_currentDir + requestItem))
                    {
                        PushFile(_currentDir + requestItem, handler);
                    }
                    else if (_dirReader.Exists(_currentDir + requestItem))
                    {
                        PushDir(_currentDir + requestItem, handler);
                    }
                    else
                        Error404(handler);
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

        public void PushDir(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " +
                         Encoding.ASCII.GetBytes(_webMaker.DirectoryContents(path, _dirReader, _currentDir)).Length + "\r\n\r\n");
            handler.Send(_webMaker.DirectoryContents(path, _dirReader, _currentDir));
        }

        private void PushFile(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Content-Type: application/octet-stream\r\n");
            handler.Send("Content-Disposition: attachment; filename = " + path.Remove(0, path.LastIndexOf('/') + 1) + "\r\n");
            handler.Send("Content-Length: " + _fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.SendFile(path);
        }

        private void Error404(IDataManager handler)
        {
            handler.Send("HTTP/1.1 404 Not Found\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " + Encoding.ASCII.GetBytes(_webMaker.Error404Page()).Length + "\r\n\r\n");
            handler.Send(_webMaker.Error404Page());
        }
    }
}