using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Server.Core;

namespace Server.Test
{
    public class IntergrationTestLiveDirectoryListing : IHttpServiceProcessor
    {
        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            var configManager = ConfigurationManager.AppSettings;
            if (configManager.AllKeys.Any(key =>
                requestItem.EndsWith(configManager[key]))
                || request.Contains("POST /"))
            {
                return false;
            }
            return serverProperties.CurrentDir != null &&
                   Directory
                       .Exists(serverProperties.CurrentDir
                               + requestItem.Substring(1));
        }

        public string ProcessRequest(string request,
            IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            requestItem = requestItem.Substring(1);
            var dirPage = DirectoryContents(requestItem,
                serverProperties.CurrentDir,
                serverProperties.Port);
            httpResponse.SendHeaders(new List<string>
                {
                    "HTTP/1.1 200 OK\r\n",
                    "Cache-Control: no-cache\r\n",
                    "Content-Type: text/html\r\n",
                    "Content-Length: "
                    + (Encoding.ASCII.GetByteCount(dirPage)) +
                    "\r\n\r\n"
                });
            httpResponse.SendBody(Encoding.ASCII.GetBytes(dirPage),
                                Encoding.ASCII.GetByteCount(dirPage));
            return "200 OK";
        }

        private string CleanRequest(string request)
        {
            if (request.Contains("HTTP/1.1"))
                return "/" + request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                    request.IndexOf(" HTTP/1.1", StringComparison.Ordinal) - 5)
                    .Replace("%20", " ");
            return "/" + request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                request.IndexOf(" HTTP/1.0", StringComparison.Ordinal) - 5)
                .Replace("%20", " ");
        }

        private string HtmlHeader()
        {
            var header = new StringBuilder();
            header.Append(@"<!DOCTYPE html>");
            header.Append(@"<html>");
            header.Append(@"<head><title>Vatic Server Directory Listing</title></head>");
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

        private string DirectoryContents(string dir,
            string root, int port)
        {
            var directoryContents = new StringBuilder();
            var files = Directory.GetFiles(root + dir);
            foreach (var replacedBackSlash in files.Select(file => file.Replace('\\', '/')))
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + port + "/" +
                                         replacedBackSlash.Replace(" ", "%20")
                                             .Remove(replacedBackSlash.IndexOf(root, StringComparison.Ordinal),
                                                 replacedBackSlash.IndexOf(root, StringComparison.Ordinal) + root.Length) +
                                         @""" >" +
                                         replacedBackSlash.Remove(0, replacedBackSlash.LastIndexOf('/') + 1)
                                         + "</a>");
            }
            var subDirs = Directory.GetDirectories(root + dir);
            foreach (var replacedBackSlash in subDirs.Select(subDir => subDir.Replace('\\', '/')))
            {
                directoryContents.Append(@"<br><a href=""http://localhost:" + port + "/" +
                                         replacedBackSlash.Replace(" ", "%20")
                                             .Remove(replacedBackSlash.IndexOf(root, StringComparison.Ordinal),
                                                 replacedBackSlash.IndexOf(root, StringComparison.Ordinal) + root.Length) +
                                         @""" >" +
                                         replacedBackSlash.Remove(0, replacedBackSlash.LastIndexOf('/') + 1)
                                         + "</a>");
            }
            return HtmlHeader() + directoryContents + HtmlTail();
        }
    }
}