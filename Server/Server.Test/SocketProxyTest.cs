using System.Net;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class SocketProxyTest
    {
        [Fact]
        public void Make_Web_Request()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 4321);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new HelloWorldServer(manager, new WebPageMaker());
            new Thread(() => RunServer(testingServer)).Start();


            var wrGeturl = WebRequest.Create("http://localhost:4321");

            wrGeturl.GetResponse();
        }

        [Fact]
        public void Make_Web_Request_For_File()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 54321);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new DirectoryServer(manager, new WebPageMaker(54321), "C:/", new DirectoryProxy(),
                new FileProxy());
            new Thread(() => RunServer(testingServer)).Start();

            var wrGeturl = WebRequest.Create(@"http://localhost:54321/Program%20Files%20(x86)/Internet%20Explorer/ie9props.propdesc");

            wrGeturl.GetResponse();
        }

        public void RunServer(IMainServer server)
        {
            server.Run();
        }
    }
}