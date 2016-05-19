using Server.Core;
using System.Text;
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
            Assert.NotNull(new DirectoryServer(mockDataMangaer, webMaker, "", new MockDirectoryProxy(), new MockFileProxy()));
        }
        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            var dirServer = new DirectoryServer(mockDataMangaer, webMaker, "", new MockDirectoryProxy(), new MockFileProxy());
            Assert.Equal(true, dirServer.stillAlive());
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET / HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(@"Home", mockRead)).Length + "\r\n\r\n");
            dataManager.VerifySend(webMaker.directoryContents(@"Home", mockRead));
            dataManager.VerifyClose();
        }
        

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_A_File()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" })
                .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(new byte[]{ 1, 2} );
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET /NotHome.txt HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: application/text\r\n");
            dataManager.VerifySend("Content-Disposition: attachment; filename = NotHome.txt\r\n");
            dataManager.VerifySend("Content-Length: " + mockFileReader.ReadAllBytes("NotHome.txt").Length + "\r\n\r\n");
            dataManager.VerifySendFile("NotHome.txt");
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" })
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] { 1, 2 });
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET /DirNotHome HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(@"DirNotHome", mockRead)).Length + "\r\n\r\n");
            dataManager.VerifySend(webMaker.directoryContents(@"DirNotHome", mockRead));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output_With_Space()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" })
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] { 1, 2 });
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET /DirNot%20Home HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(@"DirNot Home", mockRead)).Length + "\r\n\r\n");
            dataManager.VerifySend(webMaker.directoryContents(@"DirNot Home", mockRead));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET /dir HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.error404Page()).Length + "\r\n\r\n");
            dataManager.VerifySend(webMaker.error404Page());
            dataManager.VerifyClose();

        }
    }
}
