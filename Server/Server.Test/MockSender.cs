using System.Runtime.InteropServices;
using Moq;
using Server.Core;

namespace Server.Test
{
    public class MockSender : ISend
    {
        private readonly Mock<ISend> _mock;

        public MockSender()
        {
            _mock = new Mock<ISend>();
        }
        public string SendResponce(IZSocket handler, 
            IHttpResponse httpResponce)
        {
            return _mock.Object.SendResponce(handler,
                httpResponce);
        }

        public MockSender StubSendResponce(string statueCode)
        {
            _mock.Setup(m => m.SendResponce(It.IsAny<IZSocket>(),
                It.IsAny<IHttpResponse>())).Returns(statueCode);
            return this;
        }
    }
}