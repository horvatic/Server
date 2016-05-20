using Moq;
using Server.Core;

namespace Server.Test
{
    internal class MockFileProxy : IFileProxy
    {
        private readonly Mock<IFileProxy> _mock;

        public MockFileProxy()
        {
            _mock = new Mock<IFileProxy>();
        }

        public bool Exists(string path)
        {
            return _mock.Object.Exists(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return _mock.Object.ReadAllBytes(path);
        }

        public MockFileProxy StubExists(bool isDir)
        {
            _mock.Setup(m => m.Exists(It.IsAny<string>())).Returns(isDir);
            return this;
        }

        public void VerifyExists(string path)
        {
            _mock.Verify(m => m.Exists(path), Times.AtLeastOnce);
        }

        public MockFileProxy StubReadAllBytes(byte[] size)
        {
            _mock.Setup(m => m.ReadAllBytes(It.IsAny<string>())).Returns(size);
            return this;
        }

        public void VerifyReadAllBytes(string path)
        {
            _mock.Verify(m => m.ReadAllBytes(path), Times.AtLeastOnce);
        }
    }
}