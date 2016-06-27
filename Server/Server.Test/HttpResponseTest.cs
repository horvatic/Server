using System.Collections.Generic;
using System.Text;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class HttpResponseTest
    {
        [Fact]
        public void Make_Http_Response_Not_Null()
        {
            var respone = new HttpResponse(new MockZSocket());
            Assert.NotNull(respone);
        }

        [Fact]
        public void Send_HTTP_Header()
        {
            var mockSocket = new MockZSocket();
            var responeList = new List<string>
            {
                "Status : 200 OK\r\n",
                "Transfer-Encoding: chunked\r\n\r\n"
            };

            var respone = new HttpResponse(mockSocket);
            respone.SendHeaders(responeList);

            mockSocket
                .VerifySend(Encoding.ASCII
                    .GetBytes("Status : 200 OK\r\n"),
                    Encoding.ASCII
                        .GetByteCount("Status : 200 OK\r\n"));

            mockSocket
                .VerifySend(Encoding.ASCII
                    .GetBytes("Transfer-Encoding: chunked\r\n\r\n"),
                    Encoding.ASCII
                        .GetByteCount("Transfer-Encoding: chunked\r\n\r\n"));
        }

        [Fact]
        public void Send_HTTP_Body()
        {
            var data = new byte[200];
            var mockSocket = new MockZSocket();

            var respone = new HttpResponse(mockSocket);
            respone.SendBody(data, data.Length);
            mockSocket.Send(data, data.Length);
        }
    }
}