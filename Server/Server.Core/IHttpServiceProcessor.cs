namespace Server.Core
{
    public interface IHttpServiceProcessor
    {
        bool CanProcessRequest(string request,
            ServerProperties serverProperties);

        string ProcessRequest(string request, 
            IHttpResponse httpResponse, 
            ServerProperties serverProperties);
    }
}