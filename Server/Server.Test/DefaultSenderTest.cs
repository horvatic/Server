using System.Collections.Generic;
using Server.Core;
using Xunit;
namespace Server.Test
{
    public class DefaultSenderTest
    {
        [Fact]
        public void Send_HTML_Page()
        {
            var zSocket = new MockZSocket();
            var response = new HttpResponse()
            {
                Body = "Hello",
                CacheControl = "None",
                ContentType = "text/html",
                ContentLength = 20,
                HttpStatusCode = "200 OK"
            };

            var sender = new DefaultSender();
            var returnCode = sender.SendResponce(zSocket, response);

            Assert.Equal("200 OK", returnCode);
            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
            zSocket.VerifySend("Cache-Control: None\r\n");
            zSocket.VerifySend("Content-Type: text/html\r\n");
            zSocket.VerifySend("Content-Length: 20\r\n\r\n");
            zSocket.VerifySend("Hello");
        }

        [Fact]
        public void Send_Other_Headers()
        {
            var zSocket = new MockZSocket();
            var response = new HttpResponse()
            {
                OtherHeaders = new List<string>()
                {
                    "Hello : Yes"
                }
            };

            var sender = new DefaultSender();
            var returnCode = sender.SendResponce(zSocket, response);
            
            zSocket.VerifySend("Hello : Yes");
        }

        [Fact]
        public void Send_File()
        {
            var zSocket = new MockZSocket();
            var response = new HttpResponse()
            {
                CacheControl = "None",
                ContentType = "applcation/pdf",
                ContentLength = 20,
                HttpStatusCode = "201 Created",
                ContentDisposition = "Inline",
                Filename = "file",
                FilePath = "c:/file"
            };


            var sender = new DefaultSender();
            var returnCode = sender.SendResponce(zSocket, response);

            Assert.Equal("201 Created", returnCode);
            zSocket.VerifySend("HTTP/1.1 201 Created\r\n");
            zSocket.VerifySend("Cache-Control: None\r\n");
            zSocket.VerifySend("Content-Type: applcation/pdf\r\n");
            zSocket.VerifySend("Content-Disposition: Inline; " +
                "filename = file\r\n");
            zSocket.VerifySend("Content-Length: 20\r\n\r\n");
            zSocket.VerifySendFile("c:/file");

        }
    }
}
