using System;
using System.Net;

namespace Server.Core
{
    public interface ISocketProxy
    {
        bool Connected();
        void Close();
        int Send(byte[] buffer);
        void SendFile(string fileName);
        int Receive(byte[] buffer);
        void Bind(EndPoint localEp);
        void Listen(int backlog);
        ISocketProxy Accept();
    }
}