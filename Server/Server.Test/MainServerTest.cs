using Server.Core;
using System;
using Xunit;

namespace Server.Test
{

    public class MainServerTest
    {
        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null()
        {
            Assert.NotNull(new MainServer(new MockDataManager(), new WebPageMaker()));
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var dataManager = new MockDataManager()
                    .stubSentToReturn(10)
                    .stubReceive("Hello World");
            dataManager = dataManager.stubAccpetObject(dataManager);
            var server = new MainServer(dataManager, new WebPageMaker());
            server.run();

            dataManager.VerifyAccept();
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifyClose();

        }

        [Fact]
        public void Make_Sure_New_Main_Server_Does_Not_Take_Null_Socket()
        {
            Assert.Throws< SocketCanNotBeNull>( () => ( new MainServer(null, new WebPageMaker())));
        }
        [Fact]
        public void Make_Sure_New_Main_Server_Does_Not_Take_Null_WebPageMaker()
        {
            Assert.Throws<WebPageMakerCanNotBeNull>(() => (new MainServer(new MockDataManager(), null)));
        }
    }
}
