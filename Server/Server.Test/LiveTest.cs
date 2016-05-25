using System.Net;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class LiveTest
    {
        [Fact]
        public void Live_User_Presses_CTRL_C_Finsh_Request_No_New_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 55999);
            var manager = new ZSocket(endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(55999),
                null, new DirectoryProcessor(),
                new FileProcessor());
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();
            for (var i = 0; i < 100; i++)
            {
                new Thread(() => WebRequest.Create(@"http://localhost:55999//ShawnDocs/Apprenticeships/week6/LargePDFs/Greater10/h2450081.pdf").GetResponse()).Start();
            }
            
            testingServer.StopNewConn();
            testingServer.CleanUp();

            var wrFailurl = WebRequest.Create(@"http://localhost:55999//ShawnDocs/Apprenticeships/week6/LargePDFs/Greater10/h2450081.pdf");
            Assert.Throws<WebException>(() => (wrFailurl.GetResponse()));
        }

        [Fact]
        public void Make_Web_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 4321);
            //var manager = new DataManager(new SocketProxy(), endPoint);
            var manager = new ZSocket(endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(4321), null, new DirectoryProcessor(),
                new FileProcessor());
            new Thread(() => RunServerNoUntilEndRequest(testingServer)).Start();


            var wrGeturl = WebRequest.Create("http://localhost:4321");

            wrGeturl.GetResponse();
        }

        [Fact]
        public void Make_Web_Request_For_File()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 50321);
            //var manager = new DataManager(new SocketProxy(), endPoint);
            var manager = new ZSocket(endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(50321), "C:/", new DirectoryProcessor(),
                new FileProcessor());
            new Thread(() => RunServerNoUntilEndRequest(testingServer)).Start();

            var wrGeturl =
                WebRequest.Create(
                    @"http://localhost:50321/Program%20Files%20(x86)/Internet%20Explorer/ie9props.propdesc");

            wrGeturl.GetResponse();
        }

        [Fact]
        public void Make_Web_Request_For_File_Not_Accpeting_New_Connections_Hello_World()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 45418);
            var manager = new ZSocket(endPoint);
            var testingServer = new MainServer(manager, new WebPageMaker(45418),
                null, new DirectoryProcessor(),
                new FileProcessor());
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();
            var wrGeturl = WebRequest.Create(@"http://localhost:45418/");
            wrGeturl.GetResponse();
            testingServer.StopNewConn();
            testingServer.CleanUp();
            var wrFailurl = WebRequest.Create(@"http://localhost:45418/");
            Assert.Throws<WebException>(() => (wrFailurl.GetResponse()));
        }

        public void RunServerUntilEndRequest(IMainServer server)
        {
            while (server.AccectingNewConn)
            {
                server.Run();
            }
        }

        public void RunServerNoUntilEndRequest(IMainServer server)
        {
            server.Run();
            server.CleanUp();
        }
    }
}