using Moq;
using Server.Core;

namespace Server.Test
{
    public class MockServerTime : IServerTime
    {
        private readonly Mock<IServerTime> _mock;

        public MockServerTime()
        {
            _mock = new Mock<IServerTime>();
        }
        public string GetTime()
        {
            return _mock.Object.GetTime();
        }

        public MockServerTime StubTime(string time)
        {
            _mock.Setup(m => m.GetTime()).Returns(time);
            return this;
        }
    }
}
