using System.Net;
using System.Text;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class WebPageMakerTest
    {
        [Fact]
        public void Return_Web_Page_With_Names_HTML_Safe()
        {
            var maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");

            correctOutput.Append(@"First Name Submitted:<br>");
            correctOutput.Append(WebUtility.HtmlEncode("<p>John</p>") +"<br>");
            correctOutput.Append(@"Last Name Submitted:<br>");
            correctOutput.Append(WebUtility.HtmlEncode("<p>Walsher</p>") + "<br>");


            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.OutPutNames("<p>John</p>", "<p>Walsher</p>"));
        }

        [Fact]
        public void Return_Web_Page_With_Names()
        {
            var maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");

            correctOutput.Append(@"First Name Submitted:<br>");
            correctOutput.Append(@"John<br>");
            correctOutput.Append(@"Last Name Submitted:<br>");
            correctOutput.Append(@"Walsher<br>");


            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.OutPutNames("John", "Walsher"));
        }
        [Fact]
        public void Making_Web_Form_Page()
        {
            var maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");

            correctOutput.Append(@"<form action=""action_page.php"" method=""post"">");
            correctOutput.Append(@"First name:<br>");
            correctOutput.Append(@"<input type=""text"" name=""firstname""><br>");
            correctOutput.Append(@"Last name:<br>");
            correctOutput.Append(@"<input type=""text"" name=""lastname""><br><br>");
            correctOutput.Append(@"<input type=""submit"" value=""Submit"">");
            correctOutput.Append(@"</form>");


            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.NameForm());
        }

        [Fact]
        public void Making_404_Page()
        {
            var maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<h1>404</h1>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.Error404Page());
        }

        [Fact]
        public void Making_403_Page()
        {
            var maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<h1>403</h1>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.Error403Page());
        }

        [Fact]
        public void Making_Hello_World_Page()
        {
            var maker = new WebPageMaker();
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<h1>Hello World</h1>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            Assert.Equal(correctOutput.ToString(), maker.HelloWorld());
        }

        [Fact]
        public void Making_Directory_Page_With_Name_With_Spaces()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"dir 1", "dir2"})
                .StubGetFiles(new[] {"file 1", "file2", "file3"});
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
            var maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.DirectoryContents(@"Hello/World", mockRead, "Home"));
            mockRead.VerifyGetDirectories(@"Hello/World");
            mockRead.VerifyGetFiles(@"Hello/World");
        }

        [Fact]
        public void Making_Directory_Page()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
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
            var maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.DirectoryContents(@"Hello/World", mockRead, "Home"));
            mockRead.VerifyGetDirectories(@"Hello/World");
            mockRead.VerifyGetFiles(@"Hello/World");
        }

        [Fact]
        public void Making_Directory_Page_With_Slashes()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] {"dir0\\dir1", "dir0\\dir2"})
                .StubGetFiles(new[] {"dir0\\file1", "dir0\\file2", "dir0\\file3"});
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/file1"" >file1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/file2"" >file2</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/file3"" >file3</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/dir1"" >dir1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir0/dir2"" >dir2</a>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            var maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.DirectoryContents(@"Hello/World", mockRead, "Home"));
            mockRead.VerifyGetDirectories(@"Hello/World");
            mockRead.VerifyGetFiles(@"Hello/World");
        }

        [Fact]
        public void Making_Directory_Page_With_Removed_Root()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] { "dir0\\dir1", "dir0\\dir2" })
                .StubGetFiles(new[] { "dir0\\file1", "dir0\\file2", "dir0\\file3" });
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
            var maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.DirectoryContents(@"dir0/", mockRead, "dir0/"));
            mockRead.VerifyGetDirectories(@"dir0/");
            mockRead.VerifyGetFiles(@"dir0/");
        }
        [Fact]
        public void Making_Directory_Page_With_Removed_Sub_Directorys()
        {
            var mockRead = new MockDirectoryProcessor()
                .StubGetDirectories(new[] { "dir0\\dir1\\dir2\\dir1", "dir0\\dir1\\dir2\\dir2" })
                .StubGetFiles(new[] { "dir0\\dir1\\dir2\\file1", "dir0\\dir1\\dir2\\file2", "dir0\\dir1\\dir2\\file3" });
            var correctOutput = new StringBuilder();
            correctOutput.Append(@"<!DOCTYPE html>");
            correctOutput.Append(@"<html>");
            correctOutput.Append(@"<head><title>Vatic File Server</title></head>");
            correctOutput.Append(@"<body>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir1/dir2/file1"" >file1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir1/dir2/file2"" >file2</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir1/dir2/file3"" >file3</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir1/dir2/dir1"" >dir1</a>");
            correctOutput.Append(@"<br><a href=""http://localhost:8080/dir1/dir2/dir2"" >dir2</a>");
            correctOutput.Append(@"</body>");
            correctOutput.Append(@"</html>");
            var maker = new WebPageMaker(8080);
            Assert.Equal(correctOutput.ToString(), maker.DirectoryContents(@"dir0/", mockRead, "dir0/"));
            mockRead.VerifyGetDirectories(@"dir0/");
            mockRead.VerifyGetFiles(@"dir0/");
        }
    }
}