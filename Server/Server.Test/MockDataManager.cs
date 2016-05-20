using Moq;
using Server.Core;

namespace Server.Test
{
    public class MockDataManager : IDataManager
    {
        private readonly Mock<IDataManager> _mock;

        public MockDataManager()
        {
            _mock = new Mock<IDataManager>();
        }

        public bool Connected()
        {
            return _mock.Object.Connected();
        }

        public IDataManager Accept()
        {
            return _mock.Object.Accept();
        }

        public void Close()
        {
            _mock.Object.Close();
        }

        public string Receive()
        {
            return _mock.Object.Receive();
        }

        public int Send(string message)
        {
            return _mock.Object.Send(message);
        }

        public void SendFile(string message)
        {
            _mock.Object.SendFile(message);
        }

        public void VerifySend(string message)
        {
            _mock.Verify(m => m.Send(message), Times.AtLeastOnce);
        }

        public void VerifySendFile(string message)
        {
            _mock.Verify(m => m.SendFile(message), Times.AtLeastOnce);
        }

        public void VerifyAccept()
        {
            _mock.Verify(m => m.Accept(), Times.Once);
        }

        public void VerifyClose()
        {
            _mock.Verify(m => m.Close(), Times.Once);
        }

        public void VerifyReceive()
        {
            _mock.Verify(m => m.Receive(), Times.Once);
        }

        public MockDataManager StubSentToReturn(int value)
        {
            _mock.Setup(m => m.Send(It.IsAny<string>())).Returns(value);
            return this;
        }

        public MockDataManager StubConnect(bool value)
        {
            _mock.Setup(m => m.Connected()).Returns(value);
            return this;
        }

        public MockDataManager StubAccpetObject(IDataManager returnObject)
        {
            _mock.Setup(m => m.Accept()).Returns(returnObject);
            return this;
        }

        public MockDataManager StubReceive(string message)
        {
            _mock.Setup(m => m.Receive()).Returns(message);
            return this;
        }
    }
}