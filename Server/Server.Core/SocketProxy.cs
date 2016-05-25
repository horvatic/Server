using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public class SocketProxy : ISocketProxy
    {
        private readonly Socket _tcpSocket;

        public SocketProxy()
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketProxy(Socket tcpSocket)
        {
            _tcpSocket = tcpSocket;
        }
        public bool Connected()
        {
            return _tcpSocket.Connected;
        }

        public ISocketProxy Accept()
        {
            return new SocketProxy(_tcpSocket.Accept());
        }

        public void Bind(EndPoint localEp)
        {
            _tcpSocket.Bind(localEp);
        }

        public void Close()
        {
            _tcpSocket.Close();
        }

        public void Listen(int backlog)
        {
            _tcpSocket.Listen(backlog);
        }

        public int Receive(byte[] buffer)
        {
            return _tcpSocket.Receive(buffer);
        }

        public int Send(byte[] buffer)
        {
            return _tcpSocket.Send(buffer);
        }

        public void SendFile(string fileName)
        {
            _tcpSocket.SendFile(fileName);
        }
    }
}