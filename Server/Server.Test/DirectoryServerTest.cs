using System.Text;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class DirectoryServerTest
    {
        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            Assert.NotNull(new DirectoryServer(mockDataMangaer, webMaker, "", new MockDirectoryProxy(),
                new MockFileProxy()));
        }

        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            var dirServer = new DirectoryServer(mockDataMangaer, webMaker, "", new MockDirectoryProxy(),
                new MockFileProxy());
            Assert.Equal(true, dirServer.StillAlive);
        }

        [Fact]
        public void Make_Web_Server_LIVE_SOCKETS()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new DataManager(new SocketProxy());
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home", mockRead, "Home")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"Home", mockRead, "Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Blank_Request()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new string[] {})
                .StubGetFiles(new string[] {});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_A_File()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome.txt HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            mockFileReader.VerifyExists("Home/NotHome.txt");
            mockFileReader.VerifyReadAllBytes("Home/NotHome.txt");
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: application/octet-stream\r\n");
            dataManager.VerifySend("Content-Disposition: attachment; filename = NotHome.txt\r\n");
            dataManager.VerifySend("Content-Length: " + mockFileReader.ReadAllBytes("Home/NotHome.txt").Length +
                                   "\r\n\r\n");
            dataManager.VerifySendFile("Home/NotHome.txt");
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /DirNotHome HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);

            dataManager.VerifyReceive();
            mockRead.VerifyExists("Home/DirNotHome");
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home/DirNotHome", mockRead,
                                       "Home")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"Home/DirNotHome", mockRead, "Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Start_Run_Correctly()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /DirNotHome HTTP/1.1");
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.Run();
            dataManager.VerifyAccept();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output_With_Space()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /DirNot%20Home HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"DirNot Home", mockRead, "Home"))
                                       .Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"DirNot Home", mockRead, "Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /dir HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.Error404Page());
            dataManager.VerifyClose();
        }
    }
}