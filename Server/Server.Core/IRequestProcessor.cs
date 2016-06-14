namespace Server.Core
{
    public interface IRequestProcessor
    {
        string HandleRequest(string request, IZSocket handler,
            ISend sender, 
            IHttpServiceProcessor service,
            ServerProperties properties);
    }
}
