using System;
using Server.Core;
using Moq;
namespace Server.Test
{
    class MockMainServer : IMainServer
    {
        private readonly Mock<IMainServer> _mock;
        public MockMainServer()
        {
            this._mock = new Mock<IMainServer>();
        }
        public bool stillAlive()
        {
            return _mock.Object.stillAlive();
        }
        public void run()
        {
            _mock.Object.run();
        }
        public void VerifyRun()
        {
            _mock.Verify(m => m.run(), Times.Once);
        }
        public void VerifyStillAlive()
        {
            _mock.Verify(m => m.stillAlive(), Times.Once);
        }
        public IMainServer stubAccpetObject(IMainServer returnObject)
        {
            _mock.Setup(m => m.stillAlive()).Returns(false);
            return this;
        }
    }
}
