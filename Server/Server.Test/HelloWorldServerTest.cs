using System.Text;
using Server.Core;
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
        public void Make_Web_Server_LIVE_SOCKET()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new DataManager(new SocketProxy());
            var server = new HelloWorldServer(dataManager, webMaker);
            server.RunningProcess(dataManager);
        }

        [Fact]
        public void Make_Web_Server_Starts_Server()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, webMaker);
            server.Run();
            dataManager.VerifyAccept();
        }

        [Fact]
        public void Make_Web_Server_Blank_Request()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, webMaker);
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, webMaker);
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.HelloWorld()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.HelloWorld());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
        {
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("/Hello")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var webMaker = new WebPageMaker();
            var server = new HelloWorldServer(dataManager, new WebPageMaker());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.Error404Page());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Sure_Server_Is_Always_Alive()
        {
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("/Hello");
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, new WebPageMaker());

            Assert.Equal(true, server.StillAlive);
        }
    }
}