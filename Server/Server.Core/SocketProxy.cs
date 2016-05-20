using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public class SocketProxy : ISocketProxy
    {
        Socket tcpSocket;

        public SocketProxy()
        {
            tcpSocket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);
        }
        public bool Connected()
        {
            return tcpSocket.Connected;
        }
        public SocketProxy(Socket tcpSocket)
        {
            this.tcpSocket = tcpSocket;
        }
        public ISocketProxy Accept()
        {
            return new SocketProxy(tcpSocket.Accept());
        }

        public void Bind(EndPoint localEP)
        {
            tcpSocket.Bind(localEP);
        }

        public void Close()
        {
            tcpSocket.Close();
        }

        public void Listen(int backlog)
        {
            tcpSocket.Listen(backlog);
        }

        public int Receive(byte[] buffer)
        {
            return tcpSocket.Receive(buffer);
        }

        public int Send(byte[] buffer)
        {
            return tcpSocket.Send(buffer);
        }

        public void SendFile(string fileName)
        {
            tcpSocket.SendFile(fileName);
        }

        public void Shutdown(SocketShutdown how)
        {
            tcpSocket.Shutdown(how);
        }
    }
}
