using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Core
{
    public class DefaultZSocket : IZSocket
    {
        private const int BufferSize = 1024;
        private readonly Socket _tcpSocket;

        public DefaultZSocket(IPEndPoint localEndPoint)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(localEndPoint);
            _tcpSocket.Listen(int.MaxValue);
        }

        public DefaultZSocket(Socket tcpSocket)
        {
            _tcpSocket = tcpSocket;
        }

        public IZSocket Accept()
        {
            return new DefaultZSocket(_tcpSocket.Accept());
        }

        public void Close()
        {
            _tcpSocket.Close();
        }

        public bool Connected()
        {
            return _tcpSocket.Connected;
        }

        public int Send(byte[] packet, int size)
        {
            return
                _tcpSocket.Send(packet, size,
                    SocketFlags.None);
        }

        public string Receive()
        {
            var readData = new byte[BufferSize];
            var lengthRead = _tcpSocket.Receive(readData);
            return (Encoding.Default
                .GetString(readData)
                .Substring(0, lengthRead));
        }
    }
}