using System.Collections.Generic;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class DefaultRequestProcessorTest
    {
        [Fact]
        public void Handles_Request_Larger_Than_1024_Bytes()
        {
            var message = new Queue<string>();
            var request = "POST /iuiuh HTTP/1.1\r\n" +
                          "Host: localhost: 8080\r\n" +
                          "Connection: keep-alive\r\n" +
                          "Content-Length: 79841\r\n" +
                          "Cache-Control: max-age = 0\r\n" +
                          "Accept: text/html,application/xhtml+xml,application/xml; q=0.9,image/webp,*/*;q=0.8\r\n" +
                          "Origin: http://localhost:8080\r\n" +
                          "Upgrade-Insecure-Requests: 1\r\n" +
                          "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                          "Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n\r\n" +
                          "Referer: http://localhost:8080/upload" +
                          "Accept-Encoding: gzip, deflate" +
                          "Accept-Language: en-US,en;q=0.8";
            message.Enqueue(request);
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("------WebKitFormBoundaryGeqPPReAkwpcPO8e--\r\n");

            var zSocket = new MockZSocket()
                .StubReceiveWithQueue(message)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);

            var properties = new ServerProperties(@"Home",
                8080, new ServerTime(), new MockPrinter());
            var process = new DefaultRequestProcessor();
            var status = process.HandleRequest(request, zSocket,
                new MockHttpService()
                    .StubProcessRequest("200 OK"),
                properties, new HttpResponse(zSocket));
            Assert.Equal("200 OK", status);
            zSocket.VerifyManyReceive(11);
        }

        [Fact]
        public void Handles_Request_Larger_Than_1024_Bytes_Error_With_Service()
        {
            var message = new Queue<string>();
            var request = "POST /iuiuh HTTP/1.1\r\n" +
                          "Host: localhost: 8080\r\n" +
                          "Connection: keep-alive\r\n" +
                          "Content-Length: 79841\r\n" +
                          "Cache-Control: max-age = 0\r\n" +
                          "Accept: text/html,application/xhtml+xml,application/xml; q=0.9,image/webp,*/*;q=0.8\r\n" +
                          "Origin: http://localhost:8080\r\n" +
                          "Upgrade-Insecure-Requests: 1\r\n" +
                          "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                          "Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n\r\n" +
                          "Referer: http://localhost:8080/upload" +
                          "Accept-Encoding: gzip, deflate" +
                          "Accept-Language: en-US,en;q=0.8";
            message.Enqueue(request);
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("A");
            message.Enqueue("\r\n------WebKitFormBoundaryGeqPPReAkwpcPO8e--\r\n");

            var zSocket = new MockZSocket()
                .StubReceiveWithQueue(message)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", 8080,
                new ServerTime(), new MockPrinter());
            var process = new DefaultRequestProcessor();
            var status = process.HandleRequest(request, zSocket,
                new MockHttpService()
                    .StubProcessRequest("409 Conflict"),
                properties, new HttpResponse(zSocket));

            Assert.Equal("409 Conflict", status);
            zSocket.VerifyManyReceive(11);
        }

        [Fact]
        public void Handles_Request_Split_Half_InHeader()
        {
            var message = new Queue<string>();
            var request = "POST /upload HTTP/1.1\r\n" +
                          "Host: localhost: 8080\r\n" +
                          "Connection: keep-alive\r\n" +
                          "Content-Length: 79841\r\n" +
                          "Cache-Control: max-age = 0\r\n" +
                          "Accept: text/html,application/xhtml+xml,application/xml; q=0.9,image/webp,*/*;q=0.8\r\n" +
                          "Origin: http://localhost:8080\r\n" +
                          "Upgrade-Insecure-Requests: 1\r\n" +
                          "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                          "Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n" +
                          "Referer: http://localhost:8080/upload" +
                          "Accept-Encoding: gzip, deflate" +
                          "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
                          "------WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n" +
                          @"Content-Disposition: form-data; name=""saveLocation""" + "\r\n\r\n" +
                          "ZZZ\r\n" +
                          "------WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n" +
                          @"Content-Disposition: form-data; name=""fileToUpload""; filename=""" + "gid" + ".txt\"" +
                          "\r\n" +
                          "Content-Type: plan/txt\r\n\r\n";
            message.Enqueue(request);
            var packet = "Hello\r\n" + "------WebKitFormBoundaryGeqPPReAkwpcPO8e--\r\n";
            message.Enqueue(packet);

            var zSocket = new MockZSocket()
                .StubReceiveWithQueue(message)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var printer = new MockPrinter();
            var properties = new ServerProperties(@"Home",
                8080, new ServerTime(),
                printer);
            var processor = new DefaultRequestProcessor();

            var status = processor.HandleRequest(request,
                zSocket,
                new MockHttpService()
                    .StubProcessRequest("200 OK"),
                properties, new HttpResponse(zSocket));
            Assert.Equal("200 OK", status);
            zSocket.VerifyManyReceive(2);
        }

        [Fact]
        public void Handles_Request_Less_Than_1024_Bytes()
        {
            var message = new Queue<string>();
            var request = "POST /iuiuh HTTP/1.1\r\n" +
                          "Host: localhost: 8080\r\n" +
                          "Connection: keep-alive\r\n" +
                          "Content-Length: 79841\r\n" +
                          "Cache-Control: max-age = 0\r\n" +
                          "Accept: text/html,application/xhtml+xml,application/xml; q=0.9,image/webp,*/*;q=0.8\r\n" +
                          "Origin: http://localhost:8080\r\n" +
                          "Upgrade-Insecure-Requests: 1\r\n" +
                          "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                          "Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n\r\n" +
                          "Referer: http://localhost:8080/upload" +
                          "Accept-Encoding: gzip, deflate" +
                          "Accept-Language: en-US,en;q=0.8";
            message.Enqueue(request);
            message.Enqueue("------WebKitFormBoundaryGeqPPReAkwpcPO8e--\r\n");

            var zSocket = new MockZSocket()
                .StubReceiveWithQueue(message)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home",
                8080, new ServerTime(),
                new MockPrinter());
            var processor = new DefaultRequestProcessor();
            processor.HandleRequest(request, zSocket,
                new MockHttpService()
                    .StubProcessRequest("200 OK"),
                properties, new HttpResponse(zSocket));


            zSocket.VerifyManyReceive(2);
        }

        [Fact]
        public void Handles_Request_Less_Than_1024_Bytes_All_Given()
        {
            var request = GetRequest();

            var zSocket = new MockZSocket().StubReceive(request)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home",
                8080, new ServerTime(), new MockPrinter());
            var processor = new DefaultRequestProcessor();
            processor.HandleRequest(request, zSocket,
                new MockHttpService()
                    .StubProcessRequest("200 OK"), properties,
                new HttpResponse(zSocket));

            zSocket.VerifyManyReceive(0);
        }


        public string GetRequest()
        {
            return @"POST /sfs HTTP/1.1
Host: localhost:8080
Connection: keep-alive
Content-Length: 495
Cache-Control: max-age=0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Origin: http://localhost:8080
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36
Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryzOQ1PthdpYfXFgbU
Referer: http://localhost:8080/upload
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.8
------WebKitFormBoundaryzOQ1PthdpYfXFgbU
Content-Disposition: form-data; name=""saveLocation""
c:/ZZZ
------WebKitFormBoundaryzOQ1PthdpYfXFgbU
Content-Disposition: form-data; name=""fileToUpload""; filename=""testFile.txt""
Content-Type: text/plain
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
Hello
------WebKitFormBoundaryzOQ1PthdpYfXFgbU--" + "\r\n";
        }
    }
}