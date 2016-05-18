using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class WebPageMaker : IWebPageMaker
    {
        private readonly IDirectoryProxy _reader;
        private readonly int _port;
        public WebPageMaker()
        {
            _reader = null;
            _port = 0;
        }
        public WebPageMaker(IDirectoryProxy reader, int port)
        {
            _reader = reader;
            _port = port;
        }
        public string directoryContents(string dir)
        {
            StringBuilder directoryContents = new StringBuilder();
            var files = _reader.GetFiles(dir);
            foreach (string file in files)
            {
                directoryContents.Append("<br><a href=http://localhost:"+_port+"/" + file.Replace('\\', '/') + " download>" + file.Replace('\\', '/') + "</a>");
            }
            var subDirs = _reader.GetDirectories(dir);
            foreach (string subDir in subDirs)
            {
                directoryContents.Append("<br><a href=http://localhost:" + _port + "/" + subDir.Replace('\\', '/') + " >" + subDir.Replace('\\', '/') + "</a>");
            }

            return directoryContents.ToString();
        }

        public string helloWorld()
        {
            return "<h1>Hello World</h1>";
        }
    }
}
