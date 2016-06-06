using System;

namespace Server.Core
{
    public class InlinePngService : IHttpServiceProcessor
    {
        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            return serverProperties.CurrentDir != null &&
                   serverProperties.FileReader.Exists(serverProperties.CurrentDir + requestItem) &&
                   requestItem.EndsWith(".png");
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            httpResponse.HttpStatusCode = "200 OK";
            httpResponse.CacheControl = "no-cache";
            httpResponse.FilePath = serverProperties.CurrentDir + requestItem;
            httpResponse.Filename = requestItem.Remove(0, requestItem.LastIndexOf('/') + 1);
            httpResponse.ContentType = "image/png";
            httpResponse.ContentDisposition = "inline";
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
    }
}