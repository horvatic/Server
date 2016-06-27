using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Core
{
    public class HttpResponse : IHttpResponse
    {
        private readonly IZSocket _socket;

        public HttpResponse(IZSocket socket)
        {
            _socket = socket;
        }

        public bool SendHeaders(List<string> headers)
        {
            var sendSize =
                headers.Sum(header =>
                    _socket.Send(Encoding.ASCII.GetBytes(header),
                        Encoding.ASCII.GetByteCount(header)));
            return sendSize > 0 ? true : false;
        }

        public int SendBody(byte[] packet, int size)
        {
            return _socket.Send(packet, size);
        }
    }
}