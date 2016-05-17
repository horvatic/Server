using Server.Core;
using System;
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
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("GET / HTTP/1.1");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new HelloWorldServer(dataManager, new WebPageMaker());
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
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
            dataManager.VerifyClose();

        }

        [Fact]
        public void Make_Sure_New_Main_Server_Does_Not_Take_Null_Socket()
        {
            Assert.Throws< SocketCanNotBeNull>( () => ( new HelloWorldServer(null, new WebPageMaker())));
        }
        [Fact]
        public void Make_Sure_New_Main_Server_Does_Not_Take_Null_WebPageMaker()
        {
            Assert.Throws<WebPageMakerCanNotBeNull>(() => (new HelloWorldServer(new MockDataManager(), null)));
        }
    }
}
