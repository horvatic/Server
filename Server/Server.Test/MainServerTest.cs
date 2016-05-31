using System.Net;
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
                new MockFileProcessor(), 5555, new HttpResponse());
            var dirServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            dirServer.Run();
        }
        [Fact]
        public void Web_Server_No_Longer_Taking_Request()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties("", new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse());
            var dirServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            dirServer.StopNewConnAndCleanUp();
            dirServer.Run();
            zSocket.VerifyNoAccept();
        }
        [Fact]
        public void Active_Catch_When_Server_Is_Shuting_Down()
        {
            var server = new MainServer(null, null, null);
            server.Run();
        }

        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null_Hello_World()
        {
            var mockZSocket = new MockZSocket();
            var properties = new ServerProperties(null, new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse());
            Assert.NotNull(new MainServer(mockZSocket, properties, new HttpServiceFactory(new Service404())));
        }

        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var mockZSocket = new MockZSocket();
            var properties = new ServerProperties("", new MockDirectoryProcessor(),
                new MockFileProcessor(), 5555, new HttpResponse());
            var dirServer = new MainServer(mockZSocket, properties, new HttpServiceFactory(new Service404()));
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
            var serverProperties = new ServerProperties(null, new MockDirectoryProcessor(), new FileProcessor(), 5555, new HttpResponse());

            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var server = new MainServer(zSocket, serverProperties, new HttpServiceFactory(new Service404()));
            server.RunningProcess(zSocket);
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
            var serverProperties = new ServerProperties(null, new MockDirectoryProcessor(), new FileProcessor(), 5555, new HttpResponse());
            var server = new MainServer(zSocket, serverProperties, new HttpServiceFactory(new Service404()));
            server.RunningProcess(zSocket);
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
                .StubGetDirectories(new[] { "Home/dir 1", "Home/dir2" })
                .StubGetFiles(new[] { "Home/file 1", "Home/file2", "Home/file3" })
                .StubExists(true);
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive(getRequest)
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", mockRead,
               new MockFileProcessor(), 8080, new HttpResponse());
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            server.RunningProcess(zSocket);

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
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" })
                .StubExists(false);
            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(new byte[] { 1, 2 });
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties(@"Home", mockRead,
               mockFileReader, 8080, new HttpResponse());
            var server = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            server.RunningProcess(zSocket);
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

    }
}


//        [Fact]
//        public void Make_Web_Server_LIVE_SOCKET_Hello_World()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new ZSocket(new IPEndPoint((IPAddress.Loopback), 7415));
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//        }

//        [Fact]
//        public void Make_Web_Server_Starts_Server_Hello_World()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET / HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.Run();
//            zSocket.VerifyAccept();
//        }

//        [Fact]
//        public void Make_Web_Server_Blank_Request_Hello_World()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Hello_World()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET / HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.HelloWorld()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.HelloWorld());
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce__404_Hello_World()
//        {
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /NotHome.txt HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var webMaker = new WebPageMaker();
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 404 Not Found\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.Error404Page());
//            zSocket.VerifyClose();
//        }



//        [Fact]
//        public void Make_Sure_Server_Is_Always_Alive_Hello_World()
//        {
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /NotHome.txt HTTP/1.1");
//            var webMaker = new WebPageMaker();
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());

//            Assert.Equal(true, server.AcceptingNewConn);
//        }

//        [Fact]
//        public void Make_Sure_New_Main_Server_Is_Not_Null()
//        {
//            var mockZSocket = new MockZSocket();
//            var webMaker = new WebPageMaker();
//            Assert.NotNull(new MainServer(mockZSocket, webMaker, "", new MockDirectoryProcessor(),
//                new MockFileProcessor()));
//        }

//        [Fact]
//        public void Make_Sure_Sever_Is_Still_Alive()
//        {
//            var mockZSocket = new MockZSocket();
//            var webMaker = new WebPageMaker();
//            var dirServer = new MainServer(mockZSocket, webMaker, "", new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            Assert.Equal(true, dirServer.AcceptingNewConn);
//        }

//        [Fact]
//        public void Make_Web_Server_LIVE_SOCKETS()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"});
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new ZSocket(new IPEndPoint((IPAddress.Loopback), 9157));
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.RunningProcess(zSocket);
//        }


//        [Fact]
//        public void Web_Server_No_Longer_Taking_Request()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"});
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET / HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.StopNewConnAndCleanUp();
//            server.Run();
//            zSocket.VerifyNoAccept();
//        }
//        [Fact]
//        public void Server_Is_Shuting_Down()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"});
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET / HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home", mockRead, "Home")).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.DirectoryContents(@"Home", mockRead, "Home"));
//            zSocket.VerifyClose();
//            server.StopNewConnAndCleanUp();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Http_1_0()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] { "dir1", "dir2" })
//                .StubGetFiles(new[] { "file1", "file2", "file3" });
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET / HTTP/1.0")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home", mockRead, "Home")).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.DirectoryContents(@"Home", mockRead, "Home"));
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"});
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET / HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home", mockRead, "Home")).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.DirectoryContents(@"Home", mockRead, "Home"));
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Blank_Request()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new string[] {})
//                .StubGetFiles(new string[] {});
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_A_File()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"})
//                .StubExists(false);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /NotHome HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            mockFileReader.VerifyExists("Home/NotHome");
//            mockFileReader.VerifyReadAllBytes("Home/NotHome");
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: application/octet-stream\r\n");
//            zSocket.VerifySend("Content-Disposition: attachment; filename = NotHome\r\n");
//            zSocket.VerifySend("Content-Length: " + mockFileReader.ReadAllBytes("Home/NotHome").Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySendFile("Home/NotHome");
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_In_Deep_Subdirectory()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"})
//                .StubExists(false);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /dir0/dir1/dir2/NotHome HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            mockFileReader.VerifyExists("Home/dir0/dir1/dir2/NotHome");
//            mockFileReader.VerifyReadAllBytes("Home/dir0/dir1/dir2/NotHome");
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: application/octet-stream\r\n");
//            zSocket.VerifySend("Content-Disposition: attachment; filename = NotHome\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   mockFileReader.ReadAllBytes("Home/dir0/dir1/dir2/NotHome").Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySendFile("Home/dir0/dir1/dir2/NotHome");
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"})
//                .StubExists(true);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /DirNotHome HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, mockFileReader);
//            server.RunningProcess(zSocket);

//            zSocket.VerifyReceive();
//            mockRead.VerifyExists("Home/DirNotHome");
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home/DirNotHome", mockRead,
//                                       "Home")).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.DirectoryContents(@"Home/DirNotHome", mockRead, "Home"));
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Start_Run_Correctly()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"})
//                .StubExists(true);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /DirNotHome HTTP/1.1");
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, mockFileReader);
//            server.Run();
//            zSocket.VerifyAccept();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output_With_Space()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"})
//                .StubExists(true);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /DirNot%20Home HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"DirNot Home", mockRead, "Home"))
//                                       .Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.DirectoryContents(@"DirNot Home", mockRead, "Home"));
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] {"dir1", "dir2"})
//                .StubGetFiles(new[] {"file1", "file2", "file3"});
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /dir HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"Home", mockRead, new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 404 Not Found\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.Error404Page());
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce_403()
//        {
//            var mockRead = new MockDirectoryProcessor()
//                .StubGetDirectories(new[] { "dir1", "dir2" })
//                .StubGetFiles(new[] { "pagefile.sys" });
//            var webMaker = new WebPageMaker(8080);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /pagefile.sys HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, @"c:/", mockRead, new MockFileProcessor().StubExists(true));
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 403 Forbidden\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error403Page()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.Error403Page());
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Web_Server_Send_Web_Form()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /form HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.NameForm()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.NameForm());
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Web_Server_Send_Web_Form_Name_Back_As_HTML()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("POST /form HTTP/1.1\r\n" +
//                             "Host: localhost:8080\r\n" +
//                             "Connection: keep - alive\r\n" +
//                             "Content - Length: 33\r\n" +
//                             "Cache - Control: max - age = 0\r\n" +
//                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
//                             "Origin: http://localhost:8080\r\n" +
//                             "Upgrade-Insecure-Requests: 1\r\n" +
//                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
//                             "Content-Type: application/x-www-form-urlencoded\r\n" +
//                             "Referer: http://localhost:8080/form\r\n" +
//                             "Accept-Encoding: gzip, deflate\r\n" +
//                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
//                             "firstname=John&lastname=Walsher")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.OutPutNames("John", "Walsher")).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.OutPutNames("John", "Walsher"));
//            zSocket.VerifyClose();
//        }
//        [Fact]
//        public void Web_Server_Send_Web_Form_Name_Back_As_HTML_Speical_Chars()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("POST /form HTTP/1.1\r\n" +
//                             "Host: localhost:8080\r\n" +
//                             "Connection: keep - alive\r\n" +
//                             "Content - Length: 33\r\n" +
//                             "Cache - Control: max - age = 0\r\n" +
//                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
//                             "Origin: http://localhost:8080\r\n" +
//                             "Upgrade-Insecure-Requests: 1\r\n" +
//                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
//                             "Content-Type: application/x-www-form-urlencoded\r\n" +
//                             "Referer: http://localhost:8080/form\r\n" +
//                             "Accept-Encoding: gzip, deflate\r\n" +
//                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
//                             "firstname=%26&lastname=%26")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   Encoding.ASCII.GetBytes(webMaker.OutPutNames("&", "&")).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.OutPutNames("&", "&"));
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Web_Server_Get_Post_Cant_Process()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("POST /page.php HTTP/1.1\r\n" +
//                             "Host: localhost:8080\r\n" +
//                             "Connection: keep - alive\r\n" +
//                             "Content - Length: 33\r\n" +
//                             "Cache - Control: max - age = 0\r\n" +
//                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
//                             "Origin: http://localhost:8080\r\n" +
//                             "Upgrade-Insecure-Requests: 1\r\n" +
//                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
//                             "Content-Type: application/x-www-form-urlencoded\r\n" +
//                             "Referer: http://localhost:8080/form\r\n" +
//                             "Accept-Encoding: gzip, deflate\r\n" +
//                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
//                             "firstname=%26&lastname=%26")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 404 Not Found\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.Error404Page());
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Web_Server_Get_Odd_Request_Cant_Process()
//        {
//            var webMaker = new WebPageMaker();
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("PUT /page.php HTTP/1.1\r\n" +
//                             "Host: localhost:8080\r\n" +
//                             "Connection: keep - alive\r\n" +
//                             "Content - Length: 33\r\n" +
//                             "Cache - Control: max - age = 0\r\n" +
//                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
//                             "Origin: http://localhost:8080\r\n" +
//                             "Upgrade-Insecure-Requests: 1\r\n" +
//                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
//                             "Content-Type: application/x-www-form-urlencoded\r\n" +
//                             "Referer: http://localhost:8080/form\r\n" +
//                             "Accept-Encoding: gzip, deflate\r\n" +
//                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
//                             "firstname=%26&lastname=%26")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, null, new MockDirectoryProcessor(),
//                new MockFileProcessor());
//            server.RunningProcess(zSocket);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 404 Not Found\r\n");
//            zSocket.VerifySend("Content-Type: text/html\r\n");
//            zSocket.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySend(webMaker.Error404Page());
//            zSocket.VerifyClose();
//        }

//        //NOTE FOR LIVE TESTING USE THESE
//        /*
//        http://publibz.boulder.ibm.com/epubs/pdf/db2v75kz.pdf
//        http://www-03.ibm.com/systems/z/os/zos/library/bkserv/v2r2pdf/
//        http://www-03.ibm.com/systems/z/os/zos/library/bkserv/v2r2pdf/index.html#AZK
//        http://www-03.ibm.com/systems/z/os/zos/library/bkserv/zvsepdf/
//        http://publibz.boulder.ibm.com/epubs/pdf/aemm0a00.pdf
//        */
//        [Fact]
//        public void Web_Server_Send_PDF_Greater_Than_10_MB()
//        {
//            const string home = @"C:/PDF/";
//            const string pdf = @"largePDF.pdf";
//            var largeBtyeArray = new byte[10000001];
//            var mockRead = new MockDirectoryProcessor()
//                            .StubGetFiles(new[] { pdf })
//                            .StubExists(false);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(largeBtyeArray);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /"+ pdf +" HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, home, mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            mockFileReader.VerifyExists(home + pdf);
//            mockFileReader.VerifyReadAllBytes(home + pdf);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: application/octet-stream\r\n");
//            zSocket.VerifySend("Content-Disposition: attachment; filename = "+ pdf +"\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   mockFileReader.ReadAllBytes(home + pdf).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySendFile(home + pdf);
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Web_Server_Send_PDF_Less_Than_10_MB()
//        {
//            const string home = @"C:/PDF/";
//            const string pdf = @"smallPDF.pdf";
//            var largeBtyeArray = new byte[100];
//            var mockRead = new MockDirectoryProcessor()
//                            .StubGetFiles(new[] { pdf })
//                            .StubExists(false);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(largeBtyeArray);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /" + pdf + " HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, home, mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            mockFileReader.VerifyExists(home + pdf);
//            mockFileReader.VerifyReadAllBytes(home + pdf);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: application/pdf\r\n");
//            zSocket.VerifySend("Content-Disposition: inline; filename = " + pdf + "\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   mockFileReader.ReadAllBytes(home + pdf).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySendFile(home + pdf);
//            zSocket.VerifyClose();
//        }
//        [Fact]
//        public void Web_Server_Send_Inline_Text_File()
//        {
//            const string home = @"C:/text/";
//            const string text = @"smalltext.txt";
//            var largeBtyeArray = new byte[100];
//            var mockRead = new MockDirectoryProcessor()
//                            .StubGetFiles(new[] { text })
//                            .StubExists(false);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(largeBtyeArray);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /" + text + " HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, home, mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            mockFileReader.VerifyExists(home + text);
//            mockFileReader.VerifyReadAllBytes(home + text);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: text/plain\r\n");
//            zSocket.VerifySend("Content-Disposition: inline; filename = " + text + "\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   mockFileReader.ReadAllBytes(home + text).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySendFile(home + text);
//            zSocket.VerifyClose();
//        }

//        [Fact]
//        public void Web_Server_Send_Inline_PNG_File()
//        {
//            const string home = @"C:/text/";
//            const string png = @"smallimage.png";
//            var largeBtyeArray = new byte[100];
//            var mockRead = new MockDirectoryProcessor()
//                            .StubGetFiles(new[] { png })
//                            .StubExists(false);
//            var webMaker = new WebPageMaker(8080);
//            var mockFileReader = new MockFileProcessor().StubExists(true).StubReadAllBytes(largeBtyeArray);
//            var zSocket = new MockZSocket()
//                .StubSentToReturn(10)
//                .StubReceive("GET /" + png + " HTTP/1.1")
//                .StubConnect(true);
//            zSocket = zSocket.StubAcceptObject(zSocket);
//            var server = new MainServer(zSocket, webMaker, home, mockRead, mockFileReader);
//            server.RunningProcess(zSocket);
//            mockFileReader.VerifyExists(home + png);
//            mockFileReader.VerifyReadAllBytes(home + png);
//            zSocket.VerifyReceive();
//            zSocket.VerifySend("HTTP/1.1 200 OK\r\n");
//            zSocket.VerifySend("Content-Type: image/png\r\n");
//            zSocket.VerifySend("Content-Disposition: inline; filename = " + png + "\r\n");
//            zSocket.VerifySend("Content-Length: " +
//                                   mockFileReader.ReadAllBytes(home + png).Length +
//                                   "\r\n\r\n");
//            zSocket.VerifySendFile(home + png);
//            zSocket.VerifyClose();
//        }
//    }
//}