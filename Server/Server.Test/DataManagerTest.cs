using Xunit;
using Server.Core;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server.Test
{
    public class DataManagerTest
    {
        [Fact]
        public void New_Data_Manager_Not_Null_With_Port()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            DataManager manager = new DataManager(mockSocket, localEndPoint);
            Assert.NotNull(manager);
            mockSocket.VerifyBind(localEndPoint);
            mockSocket.VerifyListen(10);
        }

        [Fact]
        public void Data_Manager_Send_Message()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            DataManager manager = new DataManager(mockSocket, localEndPoint);

            manager.send("Hello");
            mockSocket.VerifySend(Encoding.ASCII.GetBytes("Hello"));
        }

        [Fact]
        public void Data_Manager_Close()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);

            manager.close();

            mockSocket.VerifyClose();
            mockSocket.VerifyShutdown(SocketShutdown.Both);


        }

        [Fact]
        public void Data_Manager_Accpect_Request()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            DataManager manager = new DataManager(mockSocket, localEndPoint);

            manager.accept();

            mockSocket.VerifyAccept();
        }

        [Fact]
        public void Data_Manager_Send_File()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);
            var message = "C:/Shawn/Docs";

            manager.sendFile(message);

            mockSocket.VerifySendFile(message);
        }

        [Fact]
        public void Data_Manager_Receive_File()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);
            var byteArray = new byte[8192];
            manager.receive();

            mockSocket.VerifyReceive(byteArray);
        }
    }
}
