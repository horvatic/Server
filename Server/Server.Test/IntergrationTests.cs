using System.Net;
using System.Net.Cache;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class IntergrationTests
    {

        public static bool GotRequest;

        private static async void WebPageHit()
        {
            var getUrl =
                WebRequest.Create(
                    @"http://localhost:54949/ShawnDocs/Apprenticeships/week6/LargePDFs/Greater10/aemm0a00.pdf");
            getUrl.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            var responce = getUrl.GetResponseAsync();
            await responce;
            GotRequest = true;
            var data = responce.Result.GetResponseStream();
            responce = getUrl.GetResponseAsync();
            await responce;
            data = responce.Result.GetResponseStream();
            responce.Result.Dispose();
        }

        [Fact]
        public void Live_User_Presses_CTRL_C_Finsh_Request_No_New_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 54949);
            var zSocket = new ZSocket(endPoint);
            var properties = new ServerProperties("c:/", new DirectoryProcessor(),
                new FileProcessor(), 54949, new HttpResponse(), new ServerTime(), new MockPrinter());
            var testingServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();

            new Thread(WebPageHit).Start();
            while (!GotRequest) ;

            testingServer.StopNewConnAndCleanUp();

            var wrFailurl =
                WebRequest.Create(
                    @"http://localhost:54949//ShawnDocs/Apprenticeships/week6/LargePDFs/Greater10/h2450081.pdf");
            Assert.Throws<WebException>(() => (wrFailurl.GetResponse()));
        }


        [Fact]
        public void Make_Web_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 4321);
            var zSocket = new ZSocket(endPoint);
            var properties = new ServerProperties("c:/", new DirectoryProcessor(),
                new FileProcessor(), 4321, new HttpResponse(), new ServerTime(), new MockPrinter());
            var testingServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            new Thread(() => RunServerNoUntilEndRequest(testingServer)).Start();


            var wrGeturl = WebRequest.Create("http://localhost:4321");

            wrGeturl.GetResponse();
        }

        [Fact]
        public void Make_Web_Request_For_File()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 50321);
            var zSocket = new ZSocket(endPoint);
            var properties = new ServerProperties("c:/", new DirectoryProcessor(),
                new FileProcessor(), 50321, new HttpResponse(), new ServerTime(), new MockPrinter());
            var testingServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
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
            var zSocket = new ZSocket(endPoint);
            var properties = new ServerProperties("c:/", new DirectoryProcessor(),
                new FileProcessor(), 45418, new HttpResponse(), new ServerTime(), new MockPrinter());
            var testingServer = new MainServer(zSocket, properties, new HttpServiceFactory(new Service404()));
            var testServerThread = new Thread(() => RunServerUntilEndRequest(testingServer));
            testServerThread.Start();
            var wrGeturl = WebRequest.Create(@"http://localhost:45418/");
            wrGeturl.GetResponse();
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