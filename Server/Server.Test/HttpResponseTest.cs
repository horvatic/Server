using Xunit;
using Server.Core;
namespace Server.Test
{
    public class HttpResponseTest
    {
        [Fact]
        public void Make_Http_Response_Not_Null()
        {
            var respone = new HttpResponse();
            Assert.NotNull(respone);
        }

        [Fact]
        public void Http_Default_Values_Correct()
        {
            var respone = new HttpResponse();
            Assert.Equal("200 OK", respone.HttpStatusCode);
            Assert.Equal("no-cache", respone.CacheControl);
            Assert.Equal("text/html", respone.ContentType);
            Assert.Equal(null, respone.ContentDisposition);
            Assert.Equal(null, respone.Filename);
            Assert.Equal(null, respone.FilePath);
            Assert.Equal("", respone.Body);

        }

        [Fact]
        public void Http_Change_Values_Correct()
        {
            var respone = new HttpResponse
            {
                HttpStatusCode = "404 Not Found",
                CacheControl = "no-store",
                ContentType = "application/octet-stream",
                Filename = "Hello",
                FilePath = "C:/Hello",
                ContentDisposition = "inline",
                Body = "<p>Hello</p>"
            };
            Assert.Equal("404 Not Found", respone.HttpStatusCode);
            Assert.Equal("no-store", respone.CacheControl);
            Assert.Equal("application/octet-stream", respone.ContentType);
            Assert.Equal("inline", respone.ContentDisposition);
            Assert.Equal("Hello", respone.Filename);
            Assert.Equal("C:/Hello", respone.FilePath);
            Assert.Equal("<p>Hello</p>", respone.Body);

        }
    }
}
