namespace Server.Core
{
    public interface IRequestProcessor
    {
        string HandleRequest(string request, IZSocket handler,
            IHttpServiceProcessor service,
            ServerProperties properties,
            IHttpResponse httpResponse);
    }
}
