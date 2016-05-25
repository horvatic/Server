using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Server.Core
{
    public class MainServer : IMainServer
    {
        private static int _numberOfThreads;
        private readonly string _currentDir;
        private readonly IDirectoryProxy _dirReader;
        private readonly IFileProxy _fileReader;
        private readonly IDataManager _socket;
        private readonly IWebPageMaker _webMaker;

        public MainServer(IDataManager socket, IWebPageMaker webMaker, string currentDir, IDirectoryProxy dirReader,
            IFileProxy fileReader)
        {
            _numberOfThreads = 0;
            _fileReader = fileReader;
            AccectingNewConn = true;
            _dirReader = dirReader;
            _socket = socket;
            _webMaker = webMaker;
            if (currentDir == null)
                _currentDir = null;
            else
                _currentDir = currentDir.EndsWith("/") ? currentDir : currentDir + "/";
        }

        public void StopNewConn()
        {
            AccectingNewConn = false;
        }

        public bool AccectingNewConn { get; private set; }

        public void CleanUp()
        {
            while (_numberOfThreads != 0)
            {
            }
            _socket.Close();
        } 

        public void RunningProcess(IDataManager handler)
        {
            Interlocked.Increment(ref _numberOfThreads);
            try
            {
                var request = handler.Receive();
                if (request.Length == 0) return;
                if (request.Contains("GET /"))
                {
                    GetProcessor(request, handler);
                }
                else if (request.Contains("POST /"))
                {
                    PostProcessor(request, handler);
                }
                else
                {
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
                Interlocked.Decrement(ref _numberOfThreads);
            }
        }

        public void Run()
        {
            try
            {
                if (!AccectingNewConn) return;
                var handler = _socket.Accept();
                new Thread(() => RunningProcess(handler)).Start();
            }
            catch (Exception)
            {
                StopNewConn();
            }
        }

        private void FileAndDirProcessing(string requestItem, IDataManager handler)
        {
            if (_currentDir != null)
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
            else
            {
                Error404(handler);
            }
        }

        private void PostProcessor(string request, IDataManager handler)
        {
            if (request.Contains("POST /action_page.php HTTP/1.1") || request.Contains("POST /action_page.php HTTP/1.0"))
            {
                var name = request.Remove(0, request.LastIndexOf("\r\n\r\n", StringComparison.Ordinal) + 4);
                var firstName = WebUtility.UrlDecode(name.Substring(0, name.IndexOf("&", StringComparison.Ordinal))
                    .Replace("firstname=", ""));
                var lastName =
                    WebUtility.UrlDecode(name.Substring(name.IndexOf("&", StringComparison.Ordinal) + 1)
                        .Replace("lastname=", ""));
                SendNames(handler, firstName, lastName);
            }
            else
            {
                Error404(handler);
            }
        }

        private string CleanRequest(string request)
        {
            if(request.Contains("HTTP/1.1"))
                return request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                        request.IndexOf(" HTTP/1.1", StringComparison.Ordinal) - 5)
                        .Replace("%20", " ");
            else
                return request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                        request.IndexOf(" HTTP/1.0", StringComparison.Ordinal) - 5)
                        .Replace("%20", " ");
        }

        private void GetProcessor(string request, IDataManager handler)
        {
            var requestItem = CleanRequest(request);

            if (request.Contains("GET / HTTP/1.1") || request.Contains("GET / HTTP/1.0"))
            {
                if (_currentDir != null)
                    HomeDir(handler);
                else
                    HelloWorld(handler);
            }
            else if (request.Contains("GET /form HTTP/1.1") || request.Contains("GET /form HTTP/1.0"))
            {
                Form(handler);
            }
            else
            {
                FileAndDirProcessing(requestItem, handler);
            }
        }

        private void SendNames(IDataManager handler, string firstName, string lastName)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " +
                         Encoding.ASCII.GetBytes(_webMaker.OutPutNames(firstName, lastName))
                             .Length +
                         "\r\n\r\n");
            handler.Send(_webMaker.OutPutNames(firstName, lastName));
        }

        private void Form(IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " +
                         Encoding.ASCII.GetBytes(_webMaker.NameForm())
                             .Length +
                         "\r\n\r\n");
            handler.Send(_webMaker.NameForm());
        }

        private void HomeDir(IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " +
                         Encoding.ASCII.GetBytes(_webMaker.DirectoryContents(_currentDir, _dirReader, _currentDir))
                             .Length +
                         "\r\n\r\n");
            handler.Send(_webMaker.DirectoryContents(_currentDir, _dirReader, _currentDir));
        }

        private void HelloWorld(IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " + Encoding.ASCII.GetBytes(_webMaker.HelloWorld()).Length +
                         "\r\n\r\n");
            handler.Send(_webMaker.HelloWorld());
        }

        private void PushDir(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: text/html\r\n");
            handler.Send("Content-Length: " +
                         Encoding.ASCII.GetBytes(_webMaker.DirectoryContents(path, _dirReader, _currentDir)).Length +
                         "\r\n\r\n");
            handler.Send(_webMaker.DirectoryContents(path, _dirReader, _currentDir));
        }

        private void PushNormalFile(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: application/octet-stream\r\n");
            handler.Send("Content-Disposition: attachment; filename = " + path.Remove(0, path.LastIndexOf('/') + 1) +
                         "\r\n");
            handler.Send("Content-Length: " + _fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.SendFile(path);
        }

        private void PushSmallPdfFile(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: application/pdf\r\n");
            handler.Send("Content-Disposition: inline; filename = " + path.Remove(0, path.LastIndexOf('/') + 1) +
                         "\r\n");
            handler.Send("Content-Length: " + _fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.SendFile(path);
        }

        private void PushTextFile(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: text/plain\r\n");
            handler.Send("Content-Disposition: inline; filename = " + path.Remove(0, path.LastIndexOf('/') + 1) +
                         "\r\n");
            handler.Send("Content-Length: " + _fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.SendFile(path);
        }

        private void PushPngFile(string path, IDataManager handler)
        {
            handler.Send("HTTP/1.1 200 OK\r\n");
            handler.Send("Cache-Control: no-cache\r\n");
            handler.Send("Content-Type: image/png\r\n");
            handler.Send("Content-Disposition: inline; filename = " + path.Remove(0, path.LastIndexOf('/') + 1) +
                         "\r\n");
            handler.Send("Content-Length: " + _fileReader.ReadAllBytes(path).Length + "\r\n\r\n");
            handler.SendFile(path);
        }

        private void PushFile(string path, IDataManager handler)
        {
            var fileSize = _fileReader.ReadAllBytes(path).Length;
            if (path.Remove(0, path.LastIndexOf('/') + 1).EndsWith(".pdf") && fileSize <= 10000000)
                PushSmallPdfFile(path, handler);
            else if (path.Remove(0, path.LastIndexOf('/') + 1).EndsWith(".txt"))
                PushTextFile(path, handler);
            else if (path.Remove(0, path.LastIndexOf('/') + 1).EndsWith(".png"))
                PushPngFile(path, handler);
            else
                PushNormalFile(path, handler);
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