using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Core
{
    public class ZSocket : IZSocket
    {
        private readonly Socket _tcpSocket;
        public ZSocket(IPEndPoint localEndPoint)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(localEndPoint);
            _tcpSocket.Listen(100);
        }

        public ZSocket(Socket tcpSocket)
        {
            _tcpSocket = tcpSocket;
        }
        public IZSocket Accept()
        {
            return new ZSocket(_tcpSocket.Accept());
        }

        public void Close()
        {
            _tcpSocket.Close();
        }

        public bool Connected()
        {
            return _tcpSocket.Connected;
        }

        public int Send(string message)
        {
            return _tcpSocket.Send(Encoding.ASCII.GetBytes(message));
        }

        public void SendFile(string message)
        {
            _tcpSocket.SendFile(message);
        }

        public string Receive()
        {
            var readData = new byte[8192];
            var lengthRead = _tcpSocket.Receive(readData);
            return (Encoding.ASCII.GetString(readData).Substring(0, lengthRead));
        }
    }
}