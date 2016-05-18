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
            Assert.NotNull(new DirectoryServer(mockDataMangaer, webMaker, ""));
        }
        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            var dirServer = new DirectoryServer(mockDataMangaer, webMaker, "");
            Assert.Equal(true, dirServer.stillAlive());
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var webMaker = new WebPageMaker(mockRead, 8080);
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET / HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home");
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.directoryContents(@"Home")).Length + "\r\n\r\n");
            dataManager.VerifySend(webMaker.directoryContents(@"Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var webMaker = new WebPageMaker(mockRead, 8080);
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET / dir");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new DirectoryServer(dataManager, webMaker, @"Home");
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes("404").Length + "\r\n\r\n");
            dataManager.VerifySend("404");
            dataManager.VerifyClose();

        }
    }
}
