using Server.Core;
using System.IO;
using Xunit;
namespace Server.Test
{
    public class FileProxyTest
    {
        [Fact]
        public void File_Not_Found()
        {
            var fileProx = new FileProxy();
            Assert.Equal(false, fileProx.Exists("efwefwefwefwefwefwefwefw"));
        }

        [Fact]
        public void File_Read()
        {
            var fileProx = new FileProxy();
            Assert.NotEmpty(fileProx.ReadAllBytes(@"C:\Program Files (x86)\Internet Explorer\ie9props.propdesc"));
        }

        [Fact]
        public void File_Not_Read()
        {
            var fileProx = new FileProxy();
            Assert.Throws<FileNotFoundException>( () => (fileProx.ReadAllBytes("wefefwefwefwefwefwefwef")));
        }
    }
}
