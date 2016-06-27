using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class IntergrationTests
    {

        [Fact]
        public void Make_Web_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 4321);
            var zSocket = new DefaultZSocket(endPoint);
            var properties = new ServerProperties("c:/",
                4321,
                new ServerTime(),
                new MockPrinter());
            var testingServer = new MainServer(zSocket,
                properties,
                new HttpServiceFactory(new IntergrationTestLiveDirectoryListing()),
                new DefaultRequestProcessor(),
                new List<string>() { "Server.Test" },
                new List<Assembly>() { Assembly.GetExecutingAssembly() });
            new Thread(() =>
                RunServerNoUntilEndRequest(testingServer)).Start();


            var wrGeturl = WebRequest.Create("http://localhost:4321");

            wrGeturl.GetResponse().GetResponseStream();
        }

        [Fact]
        public void Make_Web_Request_For_File()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 50321);
            var zSocket = new DefaultZSocket(endPoint);
            var properties = new ServerProperties("c:/",
                50321, new ServerTime(),
                new MockPrinter());
            var testingServer =
                new MainServer(zSocket, properties,
                new HttpServiceFactory(new IntergrationTestLiveFileService()),
                new DefaultRequestProcessor(),
                new List<string>() { "Server.Test" },
                new List<Assembly>() { Assembly.GetExecutingAssembly() });
            new Thread(() =>
                RunServerNoUntilEndRequest(testingServer)).Start();

            var wrGeturl =
                WebRequest.Create(
                    @"http://localhost:50321/Program%20Files%20(x86)/Internet%20Explorer/ie9props.propdesc");

            wrGeturl.GetResponse().GetResponseStream();
        }

        [Fact]
        public void Make_Web_Request_For_File_Not_Accpeting_New_Connections_Hello_World()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 45418);
            var zSocket = new DefaultZSocket(endPoint);
            var properties = new ServerProperties("c:/",
                45418, new ServerTime(),
                new MockPrinter());
            var testingServer =
                new MainServer(zSocket, properties,
                new HttpServiceFactory(new IntergrationTestLiveDirectoryListing()),
                new DefaultRequestProcessor(),
                new List<string>() { "Server.Test" },
                new List<Assembly>() { Assembly.GetExecutingAssembly() });
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();
            var wrGeturl = WebRequest.Create(@"http://localhost:45418/");
            wrGeturl.GetResponse().GetResponseStream();
            testingServer.StopNewConnAndCleanUp();
            var wrFailurl = WebRequest.Create(@"http://localhost:45418/");
            Assert.Throws<WebException>(() => (wrFailurl.GetResponse()));
        }

        public void RunServerUntilEndRequest(IMainServer server)
        {
            while (server.AcceptingNewConn)
            {
                server.Run();
            }
        }

        public void RunServerNoUntilEndRequest(IMainServer server)
        {
            server.Run();
            server.StopNewConnAndCleanUp();
        }
    }
}