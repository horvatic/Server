using Moq;
using Server.Core;

namespace Server.Test
{
    public class MockDirectoryProxy : IDirectoryProxy
    {
        private readonly Mock<IDirectoryProxy> _mock;

        public MockDirectoryProxy()
        {
            _mock = new Mock<IDirectoryProxy>();
        }

        public bool Exists(string path)
        {
            return _mock.Object.Exists(path);
        }

        public string[] GetDirectories(string path)
        {
            return _mock.Object.GetDirectories(path);
        }

        public string[] GetFiles(string path)
        {
            return _mock.Object.GetFiles(path);
        }

        public MockDirectoryProxy StubGetFiles(string[] files)
        {
            _mock.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(files);
            return this;
        }

        public MockDirectoryProxy StubGetDirectories(string[] dirs)
        {
            _mock.Setup(m => m.GetDirectories(It.IsAny<string>())).Returns(dirs);
            return this;
        }

        public MockDirectoryProxy StubExists(bool isDir)
        {
            _mock.Setup(m => m.Exists(It.IsAny<string>())).Returns(isDir);
            return this;
        }

        public void VerifyExists(string path)
        {
            _mock.Verify(m => m.Exists(path), Times.AtLeastOnce);
        }

        public void VerifyGetFiles(string path)
        {
            _mock.Verify(m => m.GetFiles(path), Times.AtLeastOnce);
        }

        public void VerifyGetDirectories(string path)
        {
            _mock.Verify(m => m.GetDirectories(path), Times.AtLeastOnce);
        }
    }
}