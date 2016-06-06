using System;
using System.Text;

namespace Server.Core
{
    public class Ftpservice : IHttpServiceProcessor
    {
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

        private string GetPath(string request)
        {
            var directory = CleanPost(request, "name=\"saveLocation\"\r\n\r\n", "\r\n");
            if (!directory.EndsWith("/"))
                directory += "/";
            var file = CleanPost(request, "filename=\"", "\"\r\n");
            return directory + file;
        }

        private string RemoveHeader(string request)
        {
            return request.Substring(request.IndexOf("\r\n\r\n"
                , StringComparison.Ordinal) + 4);
        }

        private IHttpResponse PostRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties)
        {
            var data = request.Contains("POST /upload HTTP/1.1\r\n") 
                ? RemoveHeader(request) : request;

            var path = GetPath(data);
            if (serverProperties.FileReader.Exists(path))
            {
                httpResponse.HttpStatusCode = "409 Conflict";
                return httpResponse;
            }
            var boundary = data.Substring(0, data.IndexOf("Content-Disposition: form-data;"
                , StringComparison.Ordinal)).Replace("\r\n", "");
            data = data.Substring(data.IndexOf("Content-Type: "
                , StringComparison.Ordinal));
            data = data.Substring(data.IndexOf("\r\n\r\n"
                , StringComparison.Ordinal) + 4);
            data = data.Replace(boundary + "--\r\n", "");
            serverProperties.Io.PrintToFile(data, path);
            httpResponse.HttpStatusCode = "201 Created";
            return httpResponse;
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