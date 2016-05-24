using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class DataManagerTest
    {
        [Fact]
        public void New_Data_Manager_Not_Null_With_Port()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);
            Assert.NotNull(manager);
            mockSocket.VerifyBind(localEndPoint);
            mockSocket.VerifyListen(100);
        }

        [Fact]
        public void Data_Manager_Send_Message()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);

            manager.Send("Hello");
            mockSocket.VerifySend(Encoding.ASCII.GetBytes("Hello"));
        }

        [Fact]
        public void Data_Manager_Close()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);

            manager.Close();

            mockSocket.VerifyClose();
        }

        [Fact]
        public void Data_Manager_Accpect_Request()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);

            manager.Accept();

            mockSocket.VerifyAccept();
        }

        [Fact]
        public void Data_Manager_Send_File()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);
            var message = "C:/Shawn/Docs";

            manager.SendFile(message);

            mockSocket.VerifySendFile(message);
        }

        [Fact]
        public void Data_Manager_Receive_File()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);
            var byteArray = new byte[8192];
            manager.Receive();

            mockSocket.VerifyReceive(byteArray);
        }

        [Fact]
        public void Data_Manager_Receive_File_With_Data()
        {
            var mockSocket = new MockSocketProxy();
            var localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            var manager = new DataManager(mockSocket, localEndPoint);
            var byteArray = new byte[8192];

            manager.Receive();

            mockSocket.VerifyReceive(byteArray);
        }
    }
}