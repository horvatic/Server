using Server.Core;
using Xunit;

namespace Server.Test
{
    public class FileSendServiceTest
    {
        [Fact]
        public void Make_Class_Not_Null()
        {
            Assert.NotNull(new FileSendService());
        }

        [Theory]
        [InlineData("GET /hello.exe HTTP/1.1")]
        [InlineData("GET /hi.go HTTP/1.0")]
        public void Can_Process(string getRequest)
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse());
            var fileSendService = new FileSendService();

            Assert.True(fileSendService.CanProcessRequest(getRequest, properties));
        }

        [Fact]
        public void Send_Data()
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse());
            var fileSendService = new FileSendService();

            var httpResponces = fileSendService.ProcessRequest("GET /hello.exe HTTP/1.1", new HttpResponse(), properties);

            Assert.Equal(httpResponces.FilePath, "c:/" + "hello.exe");
            Assert.Equal(httpResponces.ContentDisposition, "attachment");
            Assert.Equal(httpResponces.Filename, "hello.exe");
            Assert.Equal(httpResponces.ContentType, "application/octet-stream");
        }
    }
}