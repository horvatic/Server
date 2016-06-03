using System;
using System.IO;
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
        [InlineData("POST /upload HTTP/1.1")]
        [InlineData("POST /upload HTTP/1.0")]
        public void Can_Process(string getRequest)
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime(), new MockPrinter());
            var ftpservice = new Ftpservice();

            Assert.True(ftpservice.CanProcessRequest(getRequest, properties));
        }

        [Fact]
        public void Send_Data_Get_Request()
        {
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime(), new MockPrinter());
            var ftpservice = new Ftpservice();

            var httpResponces = ftpservice.ProcessRequest("GET /upload HTTP/1.1", new HttpResponse(), properties);

            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Upload</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<form action=""upload"" method=""post"" enctype=""multipart/form-data"">");
            correctOutput.Append(@"Select Save Location<br>");
            correctOutput.Append(@"<input type=""text"" name=""saveLocation""><br>");
            correctOutput.Append(@"Select File To Upload<br>");
            correctOutput.Append(@"<input type=""file"" name=""fileToUpload"" id=""fileToUpload""><br>");
            correctOutput.Append(@"<input type=""submit"" value=""Submit"">");
            correctOutput.Append(@"</form>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");

            Assert.Equal(correctOutput.ToString(), httpResponces.Body);
        }

        [Fact]
        public void Send_Data_Post_Request_Check_If_File_Exist_It_Dosnt()
        {
            var request = new StringBuilder();
            request.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg");
            request.Append("Content-Disposition: form-data; name=\"saveLocation\"\r\n\r\n");
            request.Append("u6t\r\n");
            request.Append("------WebKitFormBoundaryqmueWCP8RQqHnEKH\r\n");
            request.Append("Content-Disposition: form-data; name=\"fileToUpload\"; filename=\"blogBanner.png\"\r\n");
            request.Append("Content-Type: image/png\r\n\r\n?PNG\r\n");
            request.Append(
                "IHDR  s  ?   ?s   sRGB ???   gAMA  ???a   	pHYs  t  t?fx  ??IDATx^??{?%e???N?9???'N???????.E.");
            request.Append("\r\n------WebKitFormBoundaryVfPQpsTmmlrqQLLg--");
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(false);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime(), new MockPrinter());
            var ftpservice = new Ftpservice();

            var httpResponces = ftpservice.ProcessRequest(request.ToString(), new HttpResponse(), properties);

            Assert.Equal("201 Created", httpResponces.HttpStatusCode);
        }

        [Fact]
        public void Send_Data_Post_Request_Check_If_File_Exist_It_Does()
        {
            var request = new StringBuilder();
            request.Append("------WebKitFormBoundaryqmueWCP8RQqHnEKH");
            request.Append("Content-Disposition: form-data; name=\"saveLocation\"\r\n\r\n");
            request.Append("u6t\r\n");
            request.Append("------WebKitFormBoundaryqmueWCP8RQqHnEKH\r\n");
            request.Append("Content-Disposition: form-data; name=\"fileToUpload\"; filename=\"blogBanner.png\"\r\n");
            request.Append("Content-Type: image/png\r\n\r\n?PNG\r\n");
            request.Append(
                "IHDR  s  ?   ?s   sRGB ???   gAMA  ???a   	pHYs  t  t?fx  ??IDATx^??{?%e???N?9???'N???????.E.");
            request.Append("\r\n------WebKitFormBoundaryVfPQpsTmmlrqQLLg--");
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(true);
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime(), new MockPrinter());
            var ftpservice = new Ftpservice();

            var httpResponces = ftpservice.ProcessRequest(request.ToString(), new HttpResponse(), properties);

            Assert.Equal("409 Conflict", httpResponces.HttpStatusCode);
        }

        [Fact]
        public void Send_Data_Post_Request_Save_File()
        {
            var request = new StringBuilder();
            var data = new StringBuilder();
            var gid = Guid.NewGuid();
            request.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg");
            request.Append("Content-Disposition: form-data; name=\"saveLocation\"\r\n\r\n");
            request.Append("c:/\r\n");
            request.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg\r\n");
            request.Append("Content-Disposition: form-data; name=\"fileToUpload\"; filename=\"" + gid + ".txt\"\r\n");
            request.Append("Content-Type: image/png\r\n\r\n?PNG\r\n");
            data.Append(
                "Hello");
            data.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg--\r\n");
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(false);
            var io = new MockPrinter();
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime(), io);
            var ftpservice = new Ftpservice();

            ftpservice.ProcessRequest(request.ToString(), new HttpResponse(), properties);
            var httpResponces = ftpservice.ProcessRequest(data.ToString(), new HttpResponse(), properties);
            Assert.Equal("201 Created", httpResponces.HttpStatusCode);
            io.VerifyPrintToFile("?PNG\r\n", "c:/" + gid + ".txt");
            io.VerifyPrintToFile("Hello", "c:/" + gid + ".txt");
        }

        [Fact]
        public void Send_Data_Post_Request_Save_File_Small_Request()
        {
            var request = new StringBuilder();
            var gid = Guid.NewGuid();
            request.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg");
            request.Append("Content-Disposition: form-data; name=\"saveLocation\"\r\n\r\n");
            request.Append("c:/\r\n");
            request.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg\r\n");
            request.Append("Content-Disposition: form-data; name=\"fileToUpload\"; filename=\"" + gid + ".txt\"\r\n");
            request.Append("Content-Type: image/png\r\n\r\n?PNG\r\n");
            request.Append(
                "Hello");
            request.Append("------WebKitFormBoundaryVfPQpsTmmlrqQLLg--\r\n");
            var mockFileSearch = new MockFileProcessor();
            mockFileSearch.StubExists(false);
            var io = new MockPrinter();
            var properties = new ServerProperties(@"c:/",
                new MockDirectoryProcessor(), mockFileSearch, 5555, new HttpResponse(), new ServerTime(), io);
            var ftpservice = new Ftpservice();

            var httpResponces = ftpservice.ProcessRequest(request.ToString(), new HttpResponse(), properties);
            Assert.Equal("201 Created", httpResponces.HttpStatusCode);
            io.VerifyPrintToFile("?PNG\r\nHello", "c:/" + gid + ".txt");
        }
    }
}
