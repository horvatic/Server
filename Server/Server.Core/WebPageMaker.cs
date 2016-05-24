using System.Linq;
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
            var files = reader.GetFiles(dir);
            foreach (var replacedBackSlash in files.Select(file => file.Replace('\\', '/')))
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + _port + "/" +
                                         replacedBackSlash.Replace(" ", "%20").Replace(root, "") + @""" >" +
                                         replacedBackSlash.Remove(0, replacedBackSlash.LastIndexOf('/') + 1)
                                         + "</a>");
            }
            var subDirs = reader.GetDirectories(dir);
            foreach (var replacedBackSlash in subDirs.Select(subDir => subDir.Replace('\\', '/')))
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + _port + "/" +
                                         replacedBackSlash.Replace(" ", "%20").Replace(root, "") + @""" >" +
                                         replacedBackSlash.Remove(0, replacedBackSlash.LastIndexOf('/') + 1)
                                         + "</a>");
            }
            return HtmlHeader() + directoryContents + HtmlTail();
        }

        public string HelloWorld()
        {
            var helloWorldPage = new StringBuilder();
            helloWorldPage.Append(@"<h1>Hello World</h1>");
            return HtmlHeader() + helloWorldPage + HtmlTail();
        }

        public string Error404Page()
        {
            var error = new StringBuilder();
            error.Append(@"<h1>404</h1>");
            return HtmlHeader() + error + HtmlTail();
        }

        public string NameForm()
        {
            var formHtml = new StringBuilder();
            formHtml.Append(@"<form action=""action_page.php"" method=""post"">");
            formHtml.Append(@"First name:<br>");
            formHtml.Append(@"<input type=""text"" name=""firstname""><br>");
            formHtml.Append(@"Last name:<br>");
            formHtml.Append(@"<input type=""text"" name=""lastname""><br><br>");
            formHtml.Append(@"<input type=""submit"" value=""Submit"">");
            formHtml.Append(@"</form>");

            return HtmlHeader() + formHtml.ToString() + HtmlTail();
        }

        public string OutPutNames(string firstName, string lastName)
        {
            var nameOutput = new StringBuilder();
            nameOutput.Append(@"First Name Submitted:<br>");
            nameOutput.Append(firstName + "<br>");
            nameOutput.Append(@"Last Name Submitted:<br>");
            nameOutput.Append(lastName + "<br>");

            return HtmlHeader() + nameOutput.ToString() + HtmlTail();
        }
        private string HtmlHeader()
        {
            var header = new StringBuilder();
            header.Append(@"<!DOCTYPE html>");
            header.Append(@"<html>");
            header.Append(@"<head><title>Vatic File Server</title></head>");
            header.Append(@"<body>");
            return header.ToString();
        }

        private string HtmlTail()
        {
            var tail = new StringBuilder();
            tail.Append(@"</body>");
            tail.Append(@"</html>");
            return tail.ToString();
        }
    }
}