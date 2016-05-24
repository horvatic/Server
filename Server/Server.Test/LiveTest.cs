using System.Net;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class LiveTest
    {
        [Fact]
        public void Make_Web_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 4321);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(4321), null, new DirectoryProxy(),
                new FileProxy());
            new Thread(() => RunServerNoUntilEndRequest(testingServer)).Start();


            var wrGeturl = WebRequest.Create("http://localhost:4321");

            wrGeturl.GetResponse();
        }

        [Fact]
        public void Make_Web_Request_For_File()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 54321);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(54321), "C:/", new DirectoryProxy(),
                new FileProxy());
            new Thread(() => RunServerNoUntilEndRequest(testingServer)).Start();

            var wrGeturl =
                WebRequest.Create(
                    @"http://localhost:54321/Program%20Files%20(x86)/Internet%20Explorer/ie9props.propdesc");

            wrGeturl.GetResponse();
        }

        [Fact]
        public void Make_Web_Request_For_File_Not_Accpeting_New_Connections()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 65488);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(65488), "C:/", new DirectoryProxy(),
                new FileProxy());
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();
            var wrGeturl =
                WebRequest.Create(@"http://localhost:65488/Program%20Files/Internet%20Explorer/F12Resources.dll");
            new Thread(() => wrGeturl.GetResponse()).Start();
            testingServer.StopNewConn();
            var wrFailurl =
                WebRequest.Create(@"http://localhost:65488/Program%20Files/Internet%20Explorer/F12Resources.dll");
            Assert.Throws<WebException>(() => (wrFailurl.GetResponse()));
        }

        [Fact]
        public void Make_Web_Request_For_File_Not_Accpeting_New_Connections_Hello_World()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 65418);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(65418), 
                null, new DirectoryProxy(),
                new FileProxy());
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();
            var wrGeturl = WebRequest.Create(@"http://localhost:65418/");
            new Thread(() => wrGeturl.GetResponse()).Start();
            testingServer.StopNewConn();
            var wrFailurl = WebRequest.Create(@"http://localhost:65418/");
            Assert.Throws<WebException>(() => (wrFailurl.GetResponse()));
        }


        public void RunServerUntilEndRequest(IMainServer server)
        {
            while (server.StillAlive)
            {
                server.Run();
            }
        }

        public void RunServerNoUntilEndRequest(IMainServer server)
        {
            server.Run();
        }
    }
}