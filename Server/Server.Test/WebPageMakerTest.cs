using Xunit;
using Server.Core;
using System.Text;

namespace Server.Test
{
    public class WebPageMakerTest
    {
        [Fact]
        public void Making_404_Page()
        {
            WebPageMaker maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<h1>404</h1>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.error404Page());
        }
        [Fact]
        public void Making_Hello_World_Page()
        {
            WebPageMaker maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<h1>Hello World</h1>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.helloWorld());
        }

        [Fact]
        public void Making_Directory_Page_With_Name_With_Spaces()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir 1", "dir2" })
                .StubGetFiles(new[] { "file 1", "file2", "file3" });
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file%201"" >file 1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file2"" >file2</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file3"" >file3</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir%201"" >dir 1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir2"" >dir2</a>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            WebPageMaker maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.directoryContents(@"Hello/World", mockRead));
            mockRead.VerifyGetDirectories(@"Hello/World");
            mockRead.VerifyGetFiles(@"Hello/World");
        }

        [Fact]
        public void Making_Directory_Page()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new []{ "dir1", "dir2"})
                .StubGetFiles(new[] { "file1", "file2", "file3" });
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file1"" >file1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file2"" >file2</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/file3"" >file3</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir1"" >dir1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir2"" >dir2</a>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            WebPageMaker maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.directoryContents(@"Hello/World", mockRead));
            mockRead.VerifyGetDirectories(@"Hello/World");
            mockRead.VerifyGetFiles(@"Hello/World");
        }

        [Fact]
        public void Making_Directory_Page_With_Slashes()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] { "dir0\\dir1", "dir0\\dir2" })
                .StubGetFiles(new[] { "dir0\\file1", "dir0\\file2", "dir0\\file3" });
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/file1"" >dir0/file1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/file2"" >dir0/file2</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/file3"" >dir0/file3</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/dir1"" >dir0/dir1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/dir2"" >dir0/dir2</a>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            WebPageMaker maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.directoryContents(@"Hello/World", mockRead));
            mockRead.VerifyGetDirectories(@"Hello/World");
            mockRead.VerifyGetFiles(@"Hello/World");
        }
    }
}
