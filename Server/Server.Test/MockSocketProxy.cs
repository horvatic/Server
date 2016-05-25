using System.Net;
using System.Net.Sockets;
using Moq;
using Server.Core;

namespace Server.Test
{
    internal class MockSocketProxy : ISocketProxy
    {
        private readonly Mock<ISocketProxy> _mock;

        public MockSocketProxy()
        {
            _mock = new Mock<ISocketProxy>();
        }
        public void Close()
        {
            _mock.Object.Close();
        }
        public bool Connected()
        {
            return _mock.Object.Connected();
        }

        public ISocketProxy Accept()
        {
            return _mock.Object.Accept();
        }

        public void Bind(EndPoint localEp)
        {
            _mock.Object.Bind(localEp);
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

        public void VerifyBind(EndPoint localEp)
        {
            _mock.Verify(m => m.Bind(localEp));
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