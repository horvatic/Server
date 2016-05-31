using System;
using System.Text;

namespace Server.Core
{
    public class HelloWorldService : IHttpServiceProcessor
    {
        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            return ((request.Contains("GET / HTTP/1.1") || request.Contains("GET / HTTP/1.0")) && serverProperties.CurrentDir == null);
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties)
        {
            var helloWorldHtml = new StringBuilder();
            helloWorldHtml.Append(@"<!DOCTYPE html>");
            helloWorldHtml.Append(@"<html>");
            helloWorldHtml.Append(@"<head><title>Vatic Server Hello World</title></head>");
            helloWorldHtml.Append(@"<body>");
            helloWorldHtml.Append(@"<h1>Hello World</h1>");
            helloWorldHtml.Append(@"</body>");
            helloWorldHtml.Append(@"</html>");
            httpResponse.Body = helloWorldHtml.ToString();
            return httpResponse;
        }
    }
}