using System.IO;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class FileProcessorTest
    {
        [Fact]
        public void File_Not_Found()
        {
            var fileProx = new FileProcessor();
            Assert.Equal(false, fileProx.Exists("efwefwefwefwefwefwefwefw"));
        }

        [Fact]
        public void File_Read()
        {
            var fileProx = new FileProcessor();
            Assert.NotEmpty(fileProx.ReadAllBytes(@"C:\Program Files (x86)\Internet Explorer\ie9props.propdesc"));
        }

        [Fact]
        public void File_Not_Read()
        {
            var fileProx = new FileProcessor();
            Assert.Throws<FileNotFoundException>(() => (fileProx.ReadAllBytes("wefefwefwefwefwefwefwef")));
        }
    }
}