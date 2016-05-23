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

        public string DirectoryContents(string dir, IDirectoryProxy reader, string root)
        {
            var directoryContents = new StringBuilder();
            directoryContents.Append(@"<!DOCTYPE html>");
            directoryContents.Append(@"<html>");
            directoryContents.Append(@"<head><title>Vatic File Server</title></head>");
            directoryContents.Append(@"<body>");
            var files = reader.GetFiles(dir);
            foreach (var file in files)
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + _port + "/" +
                                         file.Replace('\\', '/').Replace(" ", "%20").Replace(root, "") + @""" >" +
                                         file.Replace('\\', '/').Replace(root, "") +
                                         "</a>");
            }
            var subDirs = reader.GetDirectories(dir);
            foreach (var subDir in subDirs)
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + _port + "/" +
                                         subDir.Replace('\\', '/').Replace(" ", "%20").Replace(root, "") + @""" >" +
                                         subDir.Replace('\\', '/') + "</a>").Replace(root, "");
            }
            directoryContents.Append(@"</body>");
            directoryContents.Append(@"</html>");
            return directoryContents.ToString();
        }

        public string HelloWorld()
        {
            var helloWorldPage = new StringBuilder();
            helloWorldPage.Append(@"<!DOCTYPE html>");
            helloWorldPage.Append(@"<html>");
            helloWorldPage.Append(@"<head><title>Vatic File Server</title></head>");
            helloWorldPage.Append(@"<body>");
            helloWorldPage.Append(@"<h1>Hello World</h1>");
            helloWorldPage.Append(@"</body>");
            helloWorldPage.Append(@"</html>");
            return helloWorldPage.ToString();
        }

        public string Error404Page()
        {
            var error = new StringBuilder();
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