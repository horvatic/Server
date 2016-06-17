using System.Text;

namespace Server.Core
{
    public class DefaultSender : ISend
    {
        public string SendResponce(IZSocket handler,
            IHttpResponse httpResponce)
        {
            handler.Send("HTTP/1.1 "
                         + httpResponce.HttpStatusCode + "\r\n");
            handler.Send("Cache-Control: "
                         + httpResponce.CacheControl + "\r\n");

            if (httpResponce.OtherHeaders != null)
                foreach (var header in httpResponce.OtherHeaders)
                    handler.Send(header);

            handler.Send("Content-Type: "
                         + httpResponce.ContentType + "\r\n");
            if (httpResponce.ContentDisposition != null)
            {
                handler.Send("Content-Disposition: "
                             + httpResponce.ContentDisposition + "; filename = "
                             + httpResponce.Filename +
                             "\r\n");
                handler.Send("Content-Length: "
                             + httpResponce.ContentLength +
                             "\r\n\r\n");
                handler.SendFile(httpResponce.FilePath);
            }
            else
            {
                handler.Send("Content-Length: "
                             + httpResponce.ContentLength +
                             "\r\n\r\n");
                handler.Send(httpResponce.Body);
            }
            
            return httpResponce.HttpStatusCode;
        }
    }
}