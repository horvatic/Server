using Server.Core;
using Xunit;

namespace Server.Test
{
    public class ServerPropertiesTest
    {
        [Fact]
        public void Make_Server_Properties_Not_Null()
        {
            var properties = new ServerProperties(null,
                0, new HttpResponse(), null, null);
            Assert.NotNull(properties);
        }

        [Fact]
        public void Make_Server_Properties_And_Get_All()
        {
            var currentDir = "Hello/";
            var port = 5555;
            var time = new MockServerTime();
            var io = new MockPrinter();
            var httpResponse = new HttpResponse();
            var properties = new ServerProperties(currentDir,
                port, httpResponse, time, io);
            Assert.NotNull(properties);
            Assert.Equal(currentDir, properties.CurrentDir);
            Assert.Equal(port, properties.Port);
            Assert.Equal(time, properties.Time);
            Assert.Equal(io, properties.Io);
            Assert.Equal(httpResponse, properties.DefaultResponse);
        }

        [Fact]
        public void Make_Server_Properties_Current_Dir_Null()
        {

            var port = 5555;
            var properties = new ServerProperties(null, 
                port, new HttpResponse(),
                new MockServerTime(), new MockPrinter());
            Assert.Equal(null, properties.CurrentDir);
        }

        [Fact]
        public void Make_Server_Properties_Current_No_Slash()
        {
            var port = 5555;
            var currentDir = "Hello";
            var properties = new ServerProperties(currentDir, 
                port, new HttpResponse(),
                new MockServerTime(), new MockPrinter());
            Assert.Equal(currentDir + "/", properties.CurrentDir);
        }

        [Fact]
        public void Make_Server_Properties_Persistent_Data()
        {
            var testObject = new ServerPropertiesTest();
            
            var port = 5555;
            var currentDir = "Hello";
            var properties = new ServerProperties(currentDir, 
                port, new HttpResponse(),
                new MockServerTime(), new MockPrinter(), testObject);
            Assert.Equal(testObject, properties.PersistentData);
        }
    }
}