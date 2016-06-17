using System.Collections.Generic;
using Server.Core;
using Xunit;

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

        [Fact]
        public void Http_Copy()
        {
            var respone = new HttpResponse
            {
                HttpStatusCode = "404 Not Found",
                CacheControl = "no-store",
                ContentType = "application/octet-stream",
                Filename = "Hello",
                FilePath = "C:/Hello",
                ContentDisposition = "inline",
                Body = "<p>Hello</p>",
                ContentLength = 100
           };

            var copy = new HttpResponse(respone);

            Assert.Equal("404 Not Found", copy.HttpStatusCode);
            Assert.Equal("no-store", copy.CacheControl);
            Assert.Equal("application/octet-stream", copy.ContentType);
            Assert.Equal("inline", copy.ContentDisposition);
            Assert.Equal("Hello", copy.Filename);
            Assert.Equal("C:/Hello", copy.FilePath);
            Assert.Equal("<p>Hello</p>", copy.Body);
            Assert.Equal(100, copy.ContentLength);
        }

        [Fact]
        public void Http_Clone()
        {
            var respone = new HttpResponse
            {
                HttpStatusCode = "404 Not Found",
                CacheControl = "no-store",
                ContentType = "application/octet-stream",
                Filename = "Hello",
                FilePath = "C:/Hello",
                ContentDisposition = "inline",
                Body = "<p>Hello</p>",
                ContentLength = 100
            };

            var copy = respone.Clone();

            Assert.Equal("404 Not Found", copy.HttpStatusCode);
            Assert.Equal("no-store", copy.CacheControl);
            Assert.Equal("application/octet-stream", copy.ContentType);
            Assert.Equal("inline", copy.ContentDisposition);
            Assert.Equal("Hello", copy.Filename);
            Assert.Equal("C:/Hello", copy.FilePath);
            Assert.Equal("<p>Hello</p>", copy.Body);
            Assert.Equal(100, copy.ContentLength);
        }

        [Fact]
        public void Http_Clone_Other_Headers()
        {
            var respone = new HttpResponse
            {
                HttpStatusCode = "404 Not Found",
                CacheControl = "no-store",
                ContentType = "application/octet-stream",
                Filename = "Hello",
                FilePath = "C:/Hello",
                ContentDisposition = "inline",
                Body = "<p>Hello</p>",
                ContentLength = 100,
                OtherHeaders = new List<string>()
                {
                    "Accpet : Yes"
                }
            };

            var copy = respone.Clone();

            Assert.Equal("Accpet : Yes",
                respone.OtherHeaders[0]);
        }
    }
}