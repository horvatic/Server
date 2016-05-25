using System;
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
        

        public bool AccectingNewConn => _mock.Object.AccectingNewConn;

        void IMainServer.RunningProcess(IZSocket handler)
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
        public void VerifyAccectingNewConn()
        {
            _mock.Verify(m => m.AccectingNewConn, Times.AtLeastOnce);
        }
        public void VerifyCleanUp()
        {
            _mock.Verify(m => m.CleanUp(), Times.AtLeastOnce);
        }

        public void VerifyStopNewConn()
        {
            _mock.Verify(m => m.StopNewConn(), Times.AtLeastOnce);
        }

        public MockMainServer StubAccectingNewConn()
        {
            _mock.Setup(m => m.AccectingNewConn).Returns(false);
            return this;
        }

        public void CleanUp()
        {
            _mock.Object.CleanUp();
        }
    }
}