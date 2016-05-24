using System.Text;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class MainServerTest
    {
        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null_Hello_World()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            Assert.NotNull(new MainServer(mockDataMangaer, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy()));
        }

        [Fact]
        public void Make_Web_Server_LIVE_SOCKET_Hello_World()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new DataManager(new SocketProxy());
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
        }

        [Fact]
        public void Make_Web_Server_Starts_Server_Hello_World()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.Run();
            dataManager.VerifyAccept();
        }

        [Fact]
        public void Make_Web_Server_Blank_Request_Hello_World()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Hello_World()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.HelloWorld()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.HelloWorld());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce_Hello_World()
        {
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome.txt HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var webMaker = new WebPageMaker();
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.Error404Page());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Sure_Server_Is_Always_Alive_Hello_World()
        {
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome.txt HTTP/1.1");
            var webMaker = new WebPageMaker();
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());

            Assert.Equal(true, server.StillAlive);
        }

        [Fact]
        public void Make_Sure_New_Main_Server_Is_Not_Null()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            Assert.NotNull(new MainServer(mockDataMangaer, webMaker, "", new MockDirectoryProxy(),
                new MockFileProxy()));
        }

        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var mockDataMangaer = new MockDataManager();
            var webMaker = new WebPageMaker();
            var dirServer = new MainServer(mockDataMangaer, webMaker, "", new MockDirectoryProxy(),
                new MockFileProxy());
            Assert.Equal(true, dirServer.StillAlive);
        }

        [Fact]
        public void Make_Web_Server_LIVE_SOCKETS()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new DataManager(new SocketProxy());
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
        }


        [Fact]
        public void Web_Server_No_Longer_Taking_Request()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.StopNewConn();
            server.Run();
            dataManager.VerifyNoAccept();
        }
        [Fact]
        public void Server_Is_Shuting_Down()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home", mockRead, "Home")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"Home", mockRead, "Home"));
            dataManager.VerifyClose();
            server.StopNewConn();
            Assert.Equal(false, server.StillAlive);
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home", mockRead, "Home")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"Home", mockRead, "Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Blank_Request()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new string[] {})
                .StubGetFiles(new string[] {});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_A_File()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /NotHome HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            mockFileReader.VerifyExists("Home/NotHome");
            mockFileReader.VerifyReadAllBytes("Home/NotHome");
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: application/octet-stream\r\n");
            dataManager.VerifySend("Content-Disposition: attachment; filename = NotHome\r\n");
            dataManager.VerifySend("Content-Length: " + mockFileReader.ReadAllBytes("Home/NotHome").Length +
                                   "\r\n\r\n");
            dataManager.VerifySendFile("Home/NotHome");
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_In_Deep_Subdirectory()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /dir0/dir1/dir2/NotHome HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            mockFileReader.VerifyExists("Home/dir0/dir1/dir2/NotHome");
            mockFileReader.VerifyReadAllBytes("Home/dir0/dir1/dir2/NotHome");
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: application/octet-stream\r\n");
            dataManager.VerifySend("Content-Disposition: attachment; filename = NotHome\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   mockFileReader.ReadAllBytes("Home/dir0/dir1/dir2/NotHome").Length +
                                   "\r\n\r\n");
            dataManager.VerifySendFile("Home/dir0/dir1/dir2/NotHome");
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /DirNotHome HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);

            dataManager.VerifyReceive();
            mockRead.VerifyExists("Home/DirNotHome");
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"Home/DirNotHome", mockRead,
                                       "Home")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"Home/DirNotHome", mockRead, "Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Start_Run_Correctly()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /DirNotHome HTTP/1.1");
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.Run();
            dataManager.VerifyAccept();
        }

        [Fact]
        public void Make_Web_Server_Get_Request_Send_Back_Repsonce_Not_Home_Dir_Output_With_Space()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"})
                .StubExists(true);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(false).StubReadAllBytes(new byte[] {1, 2});
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /DirNot%20Home HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.DirectoryContents(@"DirNot Home", mockRead, "Home"))
                                       .Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.DirectoryContents(@"DirNot Home", mockRead, "Home"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Make_Web_Server_Get_Bad_Request_Send_Back_Repsonce()
        {
            var mockRead = new MockDirectoryProxy()
                .StubGetDirectories(new[] {"dir1", "dir2"})
                .StubGetFiles(new[] {"file1", "file2", "file3"});
            var webMaker = new WebPageMaker(8080);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /dir HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, @"Home", mockRead, new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.Error404Page());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Web_Server_Send_Web_Form()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /form HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.NameForm()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.NameForm());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Web_Server_Send_Web_Form_Name_Back_As_HTML()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("POST /action_page.php HTTP/1.1\r\n" +
                             "Host: localhost:8080\r\n" +
                             "Connection: keep - alive\r\n" +
                             "Content - Length: 33\r\n" +
                             "Cache - Control: max - age = 0\r\n" +
                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
                             "Origin: http://localhost:8080\r\n" +
                             "Upgrade-Insecure-Requests: 1\r\n" +
                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                             "Content-Type: application/x-www-form-urlencoded\r\n" +
                             "Referer: http://localhost:8080/form\r\n" +
                             "Accept-Encoding: gzip, deflate\r\n" +
                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
                             "firstname=John&lastname=Walsher")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.OutPutNames("John", "Walsher")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.OutPutNames("John", "Walsher"));
            dataManager.VerifyClose();
        }
        [Fact]
        public void Web_Server_Send_Web_Form_Name_Back_As_HTML_Speical_Chars()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("POST /action_page.php HTTP/1.1\r\n" +
                             "Host: localhost:8080\r\n" +
                             "Connection: keep - alive\r\n" +
                             "Content - Length: 33\r\n" +
                             "Cache - Control: max - age = 0\r\n" +
                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
                             "Origin: http://localhost:8080\r\n" +
                             "Upgrade-Insecure-Requests: 1\r\n" +
                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                             "Content-Type: application/x-www-form-urlencoded\r\n" +
                             "Referer: http://localhost:8080/form\r\n" +
                             "Accept-Encoding: gzip, deflate\r\n" +
                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
                             "firstname=%26&lastname=%26")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   Encoding.ASCII.GetBytes(webMaker.OutPutNames("&", "&")).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.OutPutNames("&", "&"));
            dataManager.VerifyClose();
        }

        [Fact]
        public void Web_Server_Get_Post_Cant_Process()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("POST /page.php HTTP/1.1\r\n" +
                             "Host: localhost:8080\r\n" +
                             "Connection: keep - alive\r\n" +
                             "Content - Length: 33\r\n" +
                             "Cache - Control: max - age = 0\r\n" +
                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
                             "Origin: http://localhost:8080\r\n" +
                             "Upgrade-Insecure-Requests: 1\r\n" +
                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                             "Content-Type: application/x-www-form-urlencoded\r\n" +
                             "Referer: http://localhost:8080/form\r\n" +
                             "Accept-Encoding: gzip, deflate\r\n" +
                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
                             "firstname=%26&lastname=%26")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.Error404Page());
            dataManager.VerifyClose();
        }

        [Fact]
        public void Web_Server_Get_Odd_Request_Cant_Process()
        {
            var webMaker = new WebPageMaker();
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("PUT /page.php HTTP/1.1\r\n" +
                             "Host: localhost:8080\r\n" +
                             "Connection: keep - alive\r\n" +
                             "Content - Length: 33\r\n" +
                             "Cache - Control: max - age = 0\r\n" +
                             "Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8\r\n" +
                             "Origin: http://localhost:8080\r\n" +
                             "Upgrade-Insecure-Requests: 1\r\n" +
                             "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36\r\n" +
                             "Content-Type: application/x-www-form-urlencoded\r\n" +
                             "Referer: http://localhost:8080/form\r\n" +
                             "Accept-Encoding: gzip, deflate\r\n" +
                             "Accept-Language: en-US,en;q=0.8\r\n\r\n" +
                             "firstname=%26&lastname=%26")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());
            server.RunningProcess(dataManager);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 404 Not Found\r\n");
            dataManager.VerifySend("Content-Type: text/html\r\n");
            dataManager.VerifySend("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.Error404Page()).Length +
                                   "\r\n\r\n");
            dataManager.VerifySend(webMaker.Error404Page());
            dataManager.VerifyClose();
        }

        //NOTE FOR LIVE TESTING USE THESE
        /*
        http://publibz.boulder.ibm.com/epubs/pdf/db2v75kz.pdf
        http://www-03.ibm.com/systems/z/os/zos/library/bkserv/v2r2pdf/
        http://www-03.ibm.com/systems/z/os/zos/library/bkserv/v2r2pdf/index.html#AZK
        http://www-03.ibm.com/systems/z/os/zos/library/bkserv/zvsepdf/
        http://publibz.boulder.ibm.com/epubs/pdf/aemm0a00.pdf
        */
        [Fact]
        public void Web_Server_Send_PDF_Greater_Than_10_MB()
        {
            const string home = @"C:/PDF/";
            const string pdf = @"largePDF.pdf";
            var largeBtyeArray = new byte[10000001];
            var mockRead = new MockDirectoryProxy()
                            .StubGetFiles(new[] { pdf })
                            .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(largeBtyeArray);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /"+ pdf +" HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, home, mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            mockFileReader.VerifyExists(home + pdf);
            mockFileReader.VerifyReadAllBytes(home + pdf);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: application/octet-stream\r\n");
            dataManager.VerifySend("Content-Disposition: attachment; filename = "+ pdf +"\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   mockFileReader.ReadAllBytes(home + pdf).Length +
                                   "\r\n\r\n");
            dataManager.VerifySendFile(home + pdf);
            dataManager.VerifyClose();
        }

        [Fact]
        public void Web_Server_Send_PDF_Less_Than_10_MB()
        {
            const string home = @"C:/PDF/";
            const string pdf = @"smallPDF.pdf";
            var largeBtyeArray = new byte[100];
            var mockRead = new MockDirectoryProxy()
                            .StubGetFiles(new[] { pdf })
                            .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(largeBtyeArray);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /" + pdf + " HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, home, mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            mockFileReader.VerifyExists(home + pdf);
            mockFileReader.VerifyReadAllBytes(home + pdf);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: application/pdf\r\n");
            dataManager.VerifySend("Content-Disposition: inline; filename = " + pdf + "\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   mockFileReader.ReadAllBytes(home + pdf).Length +
                                   "\r\n\r\n");
            dataManager.VerifySendFile(home + pdf);
            dataManager.VerifyClose();
        }
        [Fact]
        public void Web_Server_Send_Inline_Text_File()
        {
            const string home = @"C:/text/";
            const string text = @"smalltext.txt";
            var largeBtyeArray = new byte[100];
            var mockRead = new MockDirectoryProxy()
                            .StubGetFiles(new[] { text })
                            .StubExists(false);
            var webMaker = new WebPageMaker(8080);
            var mockFileReader = new MockFileProxy().StubExists(true).StubReadAllBytes(largeBtyeArray);
            var dataManager = new MockDataManager()
                .StubSentToReturn(10)
                .StubReceive("GET /" + text + " HTTP/1.1")
                .StubConnect(true);
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, home, mockRead, mockFileReader);
            server.RunningProcess(dataManager);
            mockFileReader.VerifyExists(home + text);
            mockFileReader.VerifyReadAllBytes(home + text);
            dataManager.VerifyReceive();
            dataManager.VerifySend("HTTP/1.1 200 OK\r\n");
            dataManager.VerifySend("Content-Type: text/plain\r\n");
            dataManager.VerifySend("Content-Disposition: inline; filename = " + text + "\r\n");
            dataManager.VerifySend("Content-Length: " +
                                   mockFileReader.ReadAllBytes(home + text).Length +
                                   "\r\n\r\n");
            dataManager.VerifySendFile(home + text);
            dataManager.VerifyClose();
        }
    }
}