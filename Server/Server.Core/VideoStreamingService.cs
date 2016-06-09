using System;
using System.Text;

namespace Server.Core
{
    public class VideoStreamingService : IHttpServiceProcessor
    {
        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            return serverProperties.CurrentDir != null &&
                   (serverProperties.FileReader.Exists(serverProperties.CurrentDir + requestItem) &&
                    (requestItem.EndsWith(".mp4"))) || (requestItem.EndsWith(".vaticToMp4")
                                                        &&
                                                        serverProperties.FileReader.Exists(serverProperties.CurrentDir
                                                                                           +
                                                                                           requestItem.Replace(
                                                                                               ".vaticToMp4", ""))
                                                                                               && request.Contains("GET /"));
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            if (!requestItem.EndsWith(".vaticToMp4"))
            {
                httpResponse.Body = HtmlHeader() +
                                    @"<video width=""320"" height=""240"" controls>" +
                                    @"<source src=""http://127.0.0.1:" + serverProperties.Port + "/" +
                                    requestItem.Substring(1) + ".vaticToMp4" +
                                    @""" type=""video/mp4"">" +
                                    "</video>"
                                    + HtmlTail();
                httpResponse.HttpStatusCode = "200 OK";
                httpResponse.CacheControl = "no-cache";
                httpResponse.ContentType = "text/html";
            }
            else
            {
                httpResponse.FilePath = serverProperties.CurrentDir
                                        + requestItem.Substring(1).Replace(".vaticToMp4", "");
                httpResponse.Filename = requestItem.Remove(0, requestItem.LastIndexOf('/') + 1)
                    .Replace(".vaticToMp4", "");
                httpResponse.HttpStatusCode = "200 OK";
                httpResponse.CacheControl = "no-cache";
                httpResponse.ContentType = "video/mp4";
                httpResponse.ContentDisposition = "inline";
            }

            return httpResponse;
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
            header.Append(@"<head><title>Vatic Video</title></head>");
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