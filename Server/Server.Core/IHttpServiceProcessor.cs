namespace Server.Core
{
    public interface IHttpServiceProcessor
    {
        bool CanProcessRequest(string request, ServerProperties serverProperties);

        IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties);
    }
}