using Server.Core;
using Xunit;
namespace Server.Test
{
    public class ServerPropertiesTest
    {
        [Fact]
        public void Make_Server_Properties_Not_Null()
        {
            var properties = new ServerProperties(null, null, null, 0, new HttpResponse());
            Assert.NotNull(properties);
        }

        [Fact]
        public void Make_Server_Properties_And_Get_All()
        {
            var mockFileReader = new MockFileProcessor();
            var mockDirReader = new MockDirectoryProcessor();
            var currentDir = "Hello/";
            var port = 5555;
            var properties = new ServerProperties(currentDir, mockDirReader, mockFileReader, port, new HttpResponse());
            Assert.NotNull(properties);
            Assert.Equal(mockFileReader, properties.FileReader);
            Assert.Equal(mockDirReader, properties.DirReader);
            Assert.Equal(currentDir, properties.CurrentDir);
            Assert.Equal(port, properties.Port);
        }

        [Fact]
        public void Make_Server_Properties_Current_Dir_Null()
        {
            var mockFileReader = new MockFileProcessor();
            var mockDirReader = new MockDirectoryProcessor();
            var port = 5555;
            var properties = new ServerProperties(null, mockDirReader, mockFileReader, port, new HttpResponse());
            Assert.Equal(null, properties.CurrentDir);
        }
        [Fact]
        public void Make_Server_Properties_Current_No_Slash()
        {
            var mockFileReader = new MockFileProcessor();
            var mockDirReader = new MockDirectoryProcessor();
            var port = 5555;
            var currentDir = "Hello";
            var properties = new ServerProperties(currentDir, mockDirReader, mockFileReader, port, new HttpResponse());
            Assert.Equal(currentDir + "/", properties.CurrentDir);
        }
    }
}
