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

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties)
        {
            var uploadPage = new StringBuilder();
            uploadPage.Append(@"<form action=""upload"" method=""post"" enctype=""multipart/form-data"">");
            uploadPage.Append(@"Select File To Upload<br>");
            uploadPage.Append(@"<input type=""file"" name=""fileToUpload"" id=""fileToUpload""><br>");
            uploadPage.Append(@"<input type=""submit"" value=""Submit"">");
            uploadPage.Append(@"</form>");

            httpResponse.HttpStatusCode = "200 OK";
            httpResponse.CacheControl = "no-cache";
            httpResponse.ContentType = "text/html";
            httpResponse.Body = HtmlHeader() + uploadPage.ToString() + HtmlTail();

            return httpResponse;
        }

        private string CleanRequest(string request)
        {
            if (request.Contains("HTTP/1.1"))
                return request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                    request.IndexOf(" HTTP/1.1", StringComparison.Ordinal) - 5)
                    .Replace("%20", " ");
            return request.Substring(request.IndexOf("GET /", StringComparison.Ordinal) + 5,
                request.IndexOf(" HTTP/1.0", StringComparison.Ordinal) - 5)
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
