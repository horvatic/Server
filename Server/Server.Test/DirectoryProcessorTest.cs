using Server.Core;
using Xunit;

namespace Server.Test
{
    public class DirectoryProcessorTest
    {
        [Fact]
        public void Read_Directory_Contents()
        {
            var dirProxy = new DirectoryProcessor();
            Assert.NotEmpty(dirProxy.GetDirectories(@"C:/"));
            Assert.NotEmpty(dirProxy.GetFiles(@"C:/"));
        }

        [Fact]
        public void Is_A_Dir()
        {
            var dirProxy = new DirectoryProcessor();
            Assert.True(dirProxy.Exists(@"C:/"));
        }
    }
}