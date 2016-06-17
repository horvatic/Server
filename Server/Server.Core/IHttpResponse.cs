using System.Collections.Generic;

namespace Server.Core
{
    public interface IHttpResponse
    {
        string HttpStatusCode { get; set; }
        string CacheControl { get; set; }
        string ContentType { get; set; }
        string ContentDisposition { get; set; }
        string Filename { get; set; }
        string FilePath { get; set; }
        string Body { get; set; }
        long ContentLength { get; set; }

        List<string> OtherHeaders { get; set; }

        IHttpResponse Clone();
    }
}