﻿using System;
using Moq;
using Server.Core;

namespace Server.Test
{
    internal class MockFileProcessor : IFileProcessor
    {
        private readonly Mock<IFileProcessor> _mock;

        public MockFileProcessor()
        {
            _mock = new Mock<IFileProcessor>();
        }

        public bool Exists(string path)
        {
            return _mock.Object.Exists(path);
        }

        public long FileSize(string path)
        {
            if (path == "c:/pagefile.sys")
            {
                throw new Exception();
            }
            return _mock.Object.FileSize(path);
        }

        //public byte[] ReadAllBytes(string path)
        //{
        //    if (path == "c:/pagefile.sys")
        //    {
        //        throw new Exception();
        //    }
        //    return _mock.Object.ReadAllBytes(path);
        //}

        public MockFileProcessor StubExists(bool isDir)
        {
            _mock.Setup(m => m.Exists(It.IsAny<string>())).Returns(isDir);
            return this;
        }

        public void VerifyExists(string path)
        {
            _mock.Verify(m => m.Exists(path), Times.AtLeastOnce);
        }

        //public MockFileProcessor StubReadAllBytes(byte[] size)
        //{
        //    _mock.Setup(m => m.ReadAllBytes(It.IsAny<string>())).Returns(size);
        //    return this;
        //}

        public MockFileProcessor StubFileSize(long size)
        {
            _mock.Setup(m => m.FileSize(It.IsAny<string>())).Returns(size);
            return this;
        }

        public void VerifyReadAllBytes(string path)
        {
            _mock.Verify(m => m.FileSize(path), Times.AtLeastOnce);
        }

        //public void VerifyReadAllBytes(string path)
        //{
        //    _mock.Verify(m => m.ReadAllBytes(path), Times.AtLeastOnce);
        //}
    }
}