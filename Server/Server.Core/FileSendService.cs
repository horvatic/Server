using System;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Server.Core
{
    public class FileSendService : IHttpServiceProcessor
    {
        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            var configManager = ConfigurationManager.AppSettings;
            if (configManager.AllKeys.Any(key => requestItem.EndsWith(configManager[key])))
            {
                return false;
            }

            return serverProperties.CurrentDir != null &&
                   serverProperties.FileReader
                   .Exists(serverProperties.CurrentDir + requestItem.Substring(1))
                   && request.Contains("GET /");
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request).Substring(1);
            try
            {
                serverProperties.FileReader.FileSize(serverProperties.CurrentDir + requestItem);
                httpResponse.HttpStatusCode = "200 OK";
                httpResponse.CacheControl = "no-cache";
                httpResponse.FilePath = serverProperties.CurrentDir + requestItem;
                httpResponse.Filename = requestItem.Remove(0, requestItem.LastIndexOf('/') + 1);
                httpResponse.ContentType = "application/octet-stream";
                httpResponse.ContentDisposition = "attachment";
                return httpResponse;
            }
            catch (Exception)
            {
                httpResponse.HttpStatusCode = "403 Forbidden";
                httpResponse.CacheControl = "no-cache";
                httpResponse.ContentType = "text/html";
                var errorPage = new StringBuilder();
                errorPage.Append(@"<!DOCTYPE html>");
                errorPage.Append(@"<html>");
                errorPage.Append(@"<head><title>Vatic Server 403 Error Page</title></head>");
                errorPage.Append(@"<body>");
                errorPage.Append(@"<h1>403 Forbidden, Can not process request on port " + serverProperties.Port +
                                 "</h1>");
                errorPage.Append(@"</body>");
                errorPage.Append(@"</html>");
                httpResponse.Body = errorPage.ToString();
                return httpResponse;
            }
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
    }
}