using Server.Core;
using Xunit;

namespace Server.Test
{
    public class InlineTextDocServiceTest
    {
        [Fact]
        public void Make_Not_Null_Class()
        {
            Assert.NotNull(new InlineTextDocService());
        }

        [Theory]
        [InlineData("GET /hello.txt HTTP/1.1")]
        [InlineData("GET /hi.txt HTTP/1.0")]
        public void Can_Process(string getRequest)
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime());
            var inlineTextDocService = new InlineTextDocService();

            Assert.True(inlineTextDocService.CanProcessRequest(getRequest, properties));
        }

        [Theory]
        [InlineData("GET /hello.gr HTTP/1.1")]
        [InlineData("GET /hi.er HTTP/1.0")]
        public void Cant_Process(string getRequest)
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime());
            var inlineTextDocService = new InlineTextDocService();

            Assert.False(inlineTextDocService.CanProcessRequest(getRequest, properties));
        }

        [Fact]
        public void Send_Data()
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime());
            var inlineTextDocService = new InlineTextDocService();

            var httpResponces = inlineTextDocService.ProcessRequest("GET /hello.txt HTTP/1.1", new HttpResponse(), properties);

            Assert.Equal(httpResponces.FilePath, "c:/" + "hello.txt");
            Assert.Equal(httpResponces.ContentDisposition, "inline");
            Assert.Equal(httpResponces.Filename, "hello.txt");
            Assert.Equal(httpResponces.ContentType, "text/plain");
        }

    }
}
