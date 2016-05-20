using System;
using Server.Core;
using Moq;

namespace Server.Test
{
    public class MockDataManager : IDataManager
    {
        private readonly Mock<IDataManager> _mock;
        public MockDataManager()
        {
            _mock = new Mock<IDataManager>();
        }
        public bool connected()
        {
            return _mock.Object.connected();
        }
        public IDataManager accept()
        {
            return _mock.Object.accept();
        }

        public void close()
        {
            _mock.Object.close();
        }

        public string receive()
        {
            return _mock.Object.receive();
        }

        public int send(string message)
        {
            return _mock.Object.send(message);
        }

        public void sendFile(string message)
        {
            _mock.Object.sendFile(message);
        }

        public void VerifySend(string message)
        {
            _mock.Verify(m => m.send(message), Times.AtLeastOnce);
        }
        public void VerifySendFile(string message)
        {
            _mock.Verify(m => m.sendFile(message), Times.AtLeastOnce);
        }
        public void VerifyAccept()
        {
            _mock.Verify(m => m.accept(), Times.Once);
        }

        public void VerifyClose()
        {
            _mock.Verify(m => m.close(), Times.Once);
        }
        public void VerifyReceive()
        {
            _mock.Verify(m => m.receive(), Times.Once);
        }
        public MockDataManager stubSentToReturn(int value)
        {
            _mock.Setup(m => m.send(It.IsAny<string>())).Returns(value);
            return this;
        }
        public MockDataManager stubConnect(bool value)
        {
            _mock.Setup(m => m.connected()).Returns(value);
            return this;
        }

        public MockDataManager stubAccpetObject(IDataManager returnObject)
        {
            _mock.Setup(m => m.accept()).Returns(returnObject);
            return this;
        }

        public MockDataManager stubReceive(string message)
        {
            _mock.Setup(m => m.receive()).Returns(message);
            return this;
        }
    }
}
