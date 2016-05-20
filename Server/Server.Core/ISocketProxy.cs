using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public interface ISocketProxy
    {
        bool Connected();
        void Close();
        void Shutdown(SocketShutdown how);
        int Send(byte[] buffer);
        void SendFile(string fileName);
        int Receive(byte[] buffer);
        void Bind(EndPoint localEp);
        void Listen(int backlog);
        ISocketProxy Accept();

    }
}
