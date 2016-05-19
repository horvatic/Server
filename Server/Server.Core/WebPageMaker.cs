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
            directoryContents.Append(@"<!DOCTYPE html>");
            directoryContents.Append(@"<html>");
            directoryContents.Append(@"<head><title>Vatic File Server</title></head>");
            directoryContents.Append(@"<body>");
            var files = reader.GetFiles(dir);
            foreach (string file in files)
            {
                directoryContents.Append(@"<br><a href=""http://localhost:"+_port+"/" + file.Replace('\\', '/').Replace(" ", "%20") + @""" >" + file.Replace('\\', '/') + "</a>");
            }
            var subDirs = reader.GetDirectories(dir);
            foreach (string subDir in subDirs)
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + _port + "/" + subDir.Replace('\\', '/').Replace(" ", "%20") + @""" >" + subDir.Replace('\\', '/') + "</a>");
            }
            directoryContents.Append(@"</body>");
            directoryContents.Append(@"</html>");
            return directoryContents.ToString();
        }

        public string helloWorld()
        {
            StringBuilder helloWorldPage = new StringBuilder();
            helloWorldPage.Append(@"<!DOCTYPE html>");
            helloWorldPage.Append(@"<html>");
            helloWorldPage.Append(@"<head><title>Vatic File Server</title></head>");
            helloWorldPage.Append(@"<body>");
            helloWorldPage.Append(@"<h1>Hello World</h1>");
            helloWorldPage.Append(@"</body>");
            helloWorldPage.Append(@"</html>");
            return helloWorldPage.ToString();
        }

        public string error404Page()
        {
            StringBuilder error = new StringBuilder();
            error.Append(@"<!DOCTYPE html>");
            error.Append(@"<html>");
            error.Append(@"<head><title>Vatic File Server</title></head>");
            error.Append(@"<body>");
            error.Append(@"<h1>404</h1>");
            error.Append(@"</body>");
            error.Append(@"</html>");
            return error.ToString();
        }
    }
}
