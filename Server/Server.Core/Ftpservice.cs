using System;
using System.Text;

namespace Server.Core
{
    public class Ftpservice : IHttpServiceProcessor
    {
        private string _webKitFormBoundary;
        private string _writingPath;

        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            return requestItem == "upload";
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            return request.Contains("GET /") && request.IndexOf("GET /", StringComparison.Ordinal) == 0
                ? GetRequest(request, httpResponse)
                : PostRequest(request, httpResponse, serverProperties);
        }

        private IHttpResponse PostRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties)
        {
            var data = request;
            if (request.Contains("name=\"saveLocation\""))
            {
                if (GetPath(request, serverProperties))
                {
                    httpResponse.HttpStatusCode = "409 Conflict";
                    return httpResponse;
                }
                _webKitFormBoundary = request.Substring(0, request.IndexOf("Content-Disposition: form-data;"
                    , StringComparison.Ordinal)).Replace("\r\n", "");
                if (data.Contains(_webKitFormBoundary + "--\r\n"))
                {
                    data = request.Substring(data.IndexOf("Content-Type: "
                        , StringComparison.Ordinal));
                    data = data.Substring(data.IndexOf("\r\n\r\n"
                        , StringComparison.Ordinal) + 4);
                    data = data.Substring(data.IndexOf("\r\n"
                        , StringComparison.Ordinal) + 2);
                    data = data.Replace(_webKitFormBoundary + "--\r\n", "");
                    serverProperties.Io.PrintToFile(data, _writingPath);
                }
            }
            else
            {
                if (data.Contains(_webKitFormBoundary + "--\r\n"))
                {
                    data = data.Replace(_webKitFormBoundary + "--\r\n", "");
                }
                serverProperties.Io.PrintToFile(data, _writingPath);
            }
            httpResponse.HttpStatusCode = "201 Created";
            return httpResponse;
        }

        private bool GetPath(string request, ServerProperties serverProperties)
        {
            var directory = CleanPost(request, "name=\"saveLocation\"\r\n\r\n", "\r\n");
            if (!directory.EndsWith("/"))
                directory += "/";
            var file = CleanPost(request, "filename=\"", "\"\r\n");
            _writingPath = directory + file;
            return serverProperties.FileReader.Exists(_writingPath);
        }

        private string CleanPost(string request, string head, string tail)
        {
            var cleanInput = request.Substring(request.IndexOf(head
                , StringComparison.Ordinal) + head.Length);
            cleanInput = cleanInput.Remove(cleanInput.IndexOf(tail, StringComparison.Ordinal));

            return cleanInput;
        }


        private IHttpResponse GetRequest(string request, IHttpResponse httpResponse)
        {
            var uploadPage = new StringBuilder();
            uploadPage.Append(@"<form action=""upload"" method=""post"" enctype=""multipart/form-data"">");
            uploadPage.Append(@"Select Save Location<br>");
            uploadPage.Append(@"<input type=""text"" name=""saveLocation""><br>");
            uploadPage.Append(@"Select File To Upload<br>");
            uploadPage.Append(@"<input type=""file"" name=""fileToUpload"" id=""fileToUpload""><br>");
            uploadPage.Append(@"<input type=""submit"" value=""Submit"">");
            uploadPage.Append(@"</form>");

            httpResponse.HttpStatusCode = "200 OK";
            httpResponse.CacheControl = "no-cache";
            httpResponse.ContentType = "text/html";
            httpResponse.Body = HtmlHeader() + uploadPage + HtmlTail();

            return httpResponse;
        }

        private string CleanRequest(string request)
        {
            var parseVaulue = request.Contains("GET") ? "GET" : "POST";
            var offsets = request.Contains("GET") ? 5 : 6;
            if (request.Contains("HTTP/1.1"))
                return request.Substring(request.IndexOf(parseVaulue + " /", StringComparison.Ordinal) + offsets,
                    request.IndexOf(" HTTP/1.1", StringComparison.Ordinal) - offsets)
                    .Replace("%20", " ");
            return request.Substring(request.IndexOf(parseVaulue + " /", StringComparison.Ordinal) + offsets,
                request.IndexOf(" HTTP/1.0", StringComparison.Ordinal) - offsets)
                .Replace("%20", " ");
        }

        private string HtmlHeader()
        {
            var header = new StringBuilder();
            header.Append(@"<!DOCTYPE html>");
            header.Append(@"<html>");
            header.Append(@"<head><title>Vatic File Upload</title></head>");
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