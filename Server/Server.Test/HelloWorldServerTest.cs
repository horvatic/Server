using Server.Core;
using System;
using System.Text;
using Xunit;

namespace Server.Test
{

    public class HelloWorldServerTest
    {
        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null()
        {
            Assert.NotNull(new HelloWorldServer(new MockDataManager(), new WebPageMaker()));
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir1", "dir2" })
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET / HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, webMaker);
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.helloWorld()).Length + "\r\n\r\n");
            dataManager.VerifySend(webMaker.helloWorld());
            dataManager.VerifyClose();

        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
        {
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("/Hello");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, new WebPageMaker());
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes("404").Length + "\r\n\r\n");
            dataManager.VerifySend("404");
            dataManager.VerifyClose();

        }

        [Fact]
        public void Make_Sure_Server_Is_Always_Alive()
        {
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("/Hello");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, new WebPageMaker());

            Assert.Equal(true, server.stillAlive());
        }
    }
}
