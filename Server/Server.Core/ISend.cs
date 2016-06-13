using System.Security.Cryptography.X509Certificates;

namespace Server.Core
{
    public interface ISend
    {
        string SendResponce(IZSocket handler,
            IHttpResponse httpResponce);
    }
}