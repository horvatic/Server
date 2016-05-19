using System.Text;

namespace Server.Core
{
    public class WebPageMaker : IWebPageMaker
    {
        private readonly int _port;
        public WebPageMaker()
        {
            _port = 0;
        }
        public WebPageMaker(int port)
        {
            _port = port;
        }
        public string directoryContents(string dir, IDirectoryProxy reader)
        {
            StringBuilder directoryContents = new StringBuilder();
            var files = reader.GetFiles(dir);
            foreach (string file in files)
            {
                directoryContents.Append("<br><a href=http://localhost:"+_port+"/" + file.Replace('\\', '/') + " >" + file.Replace('\\', '/') + "</a>");
            }
            var subDirs = reader.GetDirectories(dir);
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
