using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;

namespace Server.Core
{
    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
        }

        public HttpResponse(IHttpResponse copy)
        {
            HttpStatusCode = copy.HttpStatusCode;

            CacheControl = copy.CacheControl;

            ContentType = copy.ContentType;

            ContentDisposition = copy.ContentDisposition;

            Filename = copy.Filename;

            FilePath = copy.FilePath;

            Body = copy.Body;

            ContentLength = copy.ContentLength;

            if (copy.OtherHeaders == null) return;
            OtherHeaders = new List<string>();
            foreach(var header in copy.OtherHeaders)
                OtherHeaders.Add(header);
        }

        public string HttpStatusCode { get; set; } = "200 OK";
        public string CacheControl { get; set; } = "no-cache";
        public string ContentType { get; set; } = "text/html";
        public string ContentDisposition { get; set; }
        public string Filename { get; set; }
        public string FilePath { get; set; }
        public string Body { get; set; } = "";
        public long ContentLength { get; set; }
        public List<string> OtherHeaders { get; set; }

        public IHttpResponse Clone()
        {
            return new HttpResponse(this);
        }
    }
}