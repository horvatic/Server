using System.Text;
using Server.Core;
using Xunit;
namespace Server.Test
{
    public class FtpserviceTest
    {
        [Fact]
        public void Make_Not_Null_Class()
        {
            Assert.NotNull(new Ftpservice());
        }

        [Theory]
        [InlineData("GET /upload HTTP/1.1")]
        [InlineData("GET /upload HTTP/1.0")]
        public void Can_Process(string getRequest)
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime());
            var ftpservice = new Ftpservice();

            Assert.True(ftpservice.CanProcessRequest(getRequest, properties));
        }

        [Fact]
        public void Send_Data_Get_Request()
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime());
            var ftpservice = new Ftpservice();

            var httpResponces = ftpservice.ProcessRequest("GET /upload HTTP/1.1", new HttpResponse(), properties);

            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Upload</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<form action=""upload"" method=""post"" enctype=""multipart/form-data"">");
            correctOutput.Append(@"Select File To Upload<br>");
            correctOutput.Append(@"<input type=""file"" name=""fileToUpload"" id=""fileToUpload""><br>");
            correctOutput.Append(@"<input type=""submit"" value=""Submit"">");
            correctOutput.Append(@"</form>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");

            Assert.Equal(correctOutput.ToString(), httpResponces.Body);
        }

    }
}
