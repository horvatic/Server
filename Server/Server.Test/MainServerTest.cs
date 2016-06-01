using System;
using System.Text;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class MainServerTest
    {
        [Fact]
        public void Get_Empty_Request()
        {
            var mockRead = new MockDirectoryProcessor();
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties("", new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse(), new ServerTime());
            var dirServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()),
                new MockPrinter());
            dirServer.Run();
        }

        [Fact]
        public void Web_Server_No_Longer_Taking_Request()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties("", new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse(), new ServerTime());
            var dirServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()),
                new MockPrinter());
            dirServer.StopNewConnAndCleanUp();
            dirServer.Run();
            zSocket.VerifyNoAccept();
        }

        [Fact]
        public void Active_Catch_When_Server_Is_Shuting_Down()
        {
            var server = new MainServer(null, null, null, new MockPrinter());
            server.Run();
        }

        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null_Hello_World()
        {
            var mockZSocket = new MockZSocket();
            var properties = new ServerProperties(null, new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse(), new ServerTime());
            Assert.NotNull(new MainServer(mockZSocket, properties, new HttpServiceFactory(new Service404()),
                new MockPrinter()));
        }

        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var mockZSocket = new MockZSocket();
            var properties = new ServerProperties("", new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse(), new ServerTime());
            var dirServer = new MainServer(mockZSocket, properties, new HttpServiceFactory(new Service404()),
                new MockPrinter());
            Assert.Equal(true, dirServer.AcceptingNewConn);
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Hello_World()
        {
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic Server Hello World</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<h1>Hello World</h1>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            var serverProperties = new ServerProperties(null, new MockDirectoryProcessor(), new FileProcessor(), 5555,
                new HttpResponse(), new ServerTime());

            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var server = new MainServer(zSocket, serverProperties, new HttpServiceFactory(new Service404()),
                new MockPrinter());
            server.RunningProcess(zSocket, Guid.NewGuid());
            zSocket.VerifyReceive();
            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
            zSocket.VerifySend("Content-Type: text/html\r\n");
            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(correctOutput.ToString()).Length +
                               "\r\n\r\n");
            zSocket.VerifySend(correctOutput.ToString());
            zSocket.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce__404_Hello_World()
        {
            var errorPage = new StringBuilder();
            errorPage.Append(@"<!DOCTYPE html>");
            errorPage.Append(@"<html>");
            errorPage.Append(@"<head><title>Vatic Server 404 Error Page</title></head>");
            errorPage.Append(@"<body>");
            errorPage.Append(@"<h1>404, Can not process request on port 5555</h1>");
            errorPage.Append(@"</body>");
            errorPage.Append(@"</html>");
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome.txt HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var serverProperties = new ServerProperties(null, new MockDirectoryProcessor(), new FileProcessor(), 5555,
                new HttpResponse(), new ServerTime());
            var server = new MainServer(zSocket, serverProperties, new HttpServiceFactory(new Service404()),
                new MockPrinter());
            server.RunningProcess(zSocket, Guid.NewGuid());
            zSocket.VerifyReceive();
            zSocket.VerifySend("HTTP/1.1 404 Not Found\r\n");
            zSocket.VerifySend("Content-Type: text/html\r\n");
            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(errorPage.ToString()).Length +
                               "\r\n\r\n");
            zSocket.VerifySend(errorPage.ToString());
            zSocket.VerifyClose();
        }

        [Theory]
        [InlineData("GET / HTTP/1.0")]
        [InlineData("GET / HTTP/1.1")]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Http(string getRequest)
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"Home/dir 1", "Home/dir2"})
                .StubGetFiles(new[] {"Home/file 1", "Home/file2", "Home/file3"})
                .StubExists(true);
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive(getRequest)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", mockRead,
                new MockFileProcessor(), 8080, new HttpResponse(), new ServerTime());
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()), new MockPrinter());
            server.RunningProcess(zSocket, Guid.NewGuid());

            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic Server Directory Listing</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file%201"" >file 1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file2"" >file2</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file3"" >file3</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir%201"" >dir 1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir2"" >dir2</a>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");

            zSocket.VerifyReceive();
            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
            zSocket.VerifySend("Content-Type: text/html\r\n");
            zSocket.VerifySend("Content-Length: " +
                               Encoding.ASCII.GetBytes(correctOutput.ToString()).Length +
                               "\r\n\r\n");
            zSocket.VerifySend(correctOutput.ToString());
            zSocket.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_A_File()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(false);
            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", mockRead,
                mockFileReader, 8080, new HttpResponse(), new ServerTime());
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()), new MockPrinter());
            server.RunningProcess(zSocket, Guid.NewGuid());
            mockFileReader.VerifyReadAllBytes("Home/NotHome");
            zSocket.VerifyReceive();
            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
            zSocket.VerifySend("Content-Type: application/octet-stream\r\n");
            zSocket.VerifySend("Content-Disposition: attachment; filename = NotHome\r\n");
            zSocket.VerifySend("Content-Length: " + mockFileReader.ReadAllBytes("Home/NotHome").Length +
                               "\r\n\r\n");
            zSocket.VerifySendFile("Home/NotHome");
            zSocket.VerifyClose();
        }

        [Fact]
        public void Log_Server_Hits()
        {
            var printLog = new MockPrinter();
            var gid = Guid.NewGuid();

            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(false);
            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var mockTime = new MockServerTime().StubTime("10am");
            var properties = new ServerProperties(@"Home", mockRead,
                mockFileReader, 8080, new HttpResponse(), mockTime);
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()), printLog);
            server.RunningProcess(zSocket, gid);
            printLog.VerifyPrint("[10am] [<" + gid + ">] GET /NotHome HTTP/1.1");
            printLog.VerifyPrint("[10am] [<" + gid + ">] 200 OK");
        }

        [Fact]
        public void Handles_Request_Larger_Than_8192_Bytes()
        {
            var request = "POST /upload HTTP/1.1\r\n" +
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


            var mockRead = new MockDirectoryProcessor();
            var mockFileReader = new MockFileProcessor();
            var zSocket = new MockZSocket().StubReceive(request)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", mockRead,
                mockFileReader, 8080, new HttpResponse(), new ServerTime());
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()), new MockPrinter());
            server.RunningProcess(zSocket, Guid.NewGuid());

            zSocket.VerifyManyReceive(11);


        }

        [Fact]
        public void Handles_Request_Less_Than_8192_Bytes()
        {
            var request = "POST /upload HTTP/1.1\r\n" +
                          "Host: localhost: 8080\r\n" +
                          "Connection: keep-alive\r\n" +
                          "Content-Length: 100\r\n" +
                          "Cache-Control: max-age = 0\r\n" +
                          "Accept: text/html,application/xhtml+xml,application/xml; q=0.9,image/webp,*/*;q=0.8\r\n" +
                          "Origin: http://localhost:8080\r\n" +
                          "Upgrade-Insecure-Requests: 1\r\n" +
                          "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                          "Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryGeqPPReAkwpcPO8e\r\n\r\n" +
                          "Referer: http://localhost:8080/upload" +
                          "Accept-Encoding: gzip, deflate" +
                          "Accept-Language: en-US,en;q=0.8";


            var mockRead = new MockDirectoryProcessor();
            var mockFileReader = new MockFileProcessor();
            var zSocket = new MockZSocket().StubReceive(request)
                .StubSentToReturn(10)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", mockRead,
                mockFileReader, 8080, new HttpResponse(), new ServerTime());
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()), new MockPrinter());
            server.RunningProcess(zSocket, Guid.NewGuid());

            zSocket.VerifyManyReceive(1);


        }
    }
}