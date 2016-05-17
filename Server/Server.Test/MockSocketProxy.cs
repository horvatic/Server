using System;
using System.Net;
using Server.Core;
using Moq;
using System.Net.Sockets;

namespace Server.Test
{
    class MockSocketProxy : ISocketProxy
    {

        private readonly Mock<ISocketProxy> _mock;
        public void Close()
        {
            _mock.Object.Close();
        }

        public void Shutdown(SocketShutdown how)
        {
            _mock.Object.Shutdown(how);
        }
        public MockSocketProxy()
        {
            _mock = new Mock<ISocketProxy>();
        }

        public ISocketProxy Accept()
        {
            return _mock.Object.Accept();
        }
        public void Bind(EndPoint localEP)
        {
            _mock.Object.Bind(localEP);
        }

        public void Listen(int backlog)
        {
            _mock.Object.Listen(backlog);
        }

        public int Receive(byte[] buffer)
        {
            return _mock.Object.Receive(buffer);
        }

        public int Send(byte[] buffer)
        {
            return _mock.Object.Send(buffer);
        }

        public void SendFile(string fileName)
        {
            _mock.Object.SendFile(fileName);
        }

        public void VerifyBind(EndPoint localEP)
        {
            _mock.Verify(m => m.Bind(localEP));
        }

        public void VerifyListen(int backlog)
        {
            _mock.Verify(m => m.Listen(backlog));
        }

        public void VerifySend(byte[] buffer)
        {
            _mock.Verify(m => m.Send(buffer));
        }
        public void VerifyAccept()
        {
            _mock.Verify(m => m.Accept(), Times.Once);
        }

        public void VerifyClose()
        {
            _mock.Verify(m => m.Close(), Times.Once);
        }

        public void VerifyShutdown(SocketShutdown how)
        {
            _mock.Verify(m => m.Shutdown(how), Times.Once);
        }

        public void VerifySendFile(string fileName)
        {
            _mock.Verify(m => m.SendFile(fileName), Times.Once);
        }

        public void VerifyReceive(byte[] buffer)
        {
            _mock.Verify(m => m.Receive(buffer));
        }
    }
}
