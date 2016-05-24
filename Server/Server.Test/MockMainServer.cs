using Moq;
using Server.Core;

namespace Server.Test
{
    internal class MockMainServer : IMainServer
    {
        private readonly Mock<IMainServer> _mock;

        public MockMainServer()
        {
            _mock = new Mock<IMainServer>();
        }

        public void StopNewConn()
        {
            _mock.Object.StopNewConn();
        }

        public bool StillAlive => _mock.Object.StillAlive;

        void IMainServer.RunningProcess(IDataManager handler)
        {
            _mock.Object.RunningProcess(handler);
        }

        public void Run()
        {
            _mock.Object.Run();
        }

        public void VerifyRun()
        {
            _mock.Verify(m => m.Run(), Times.Once);
        }

        public void VerifyStillAlive()
        {
            _mock.Verify(m => m.StillAlive, Times.Once);
        }

        public MockMainServer StubStillAlive()
        {
            _mock.Setup(m => m.StillAlive).Returns(false);
            return this;
        }
    }
}