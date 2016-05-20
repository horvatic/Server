using Server.Core;
using System.IO;
using System.Net;
using System.Threading;
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
            new Thread(() => runServer(testingServer)).Start();

            string sURL;
            sURL = "http://localhost:4321";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            wrGETURL.GetResponse();

        }

        [Fact]
        public void Make_Web_Request_For_File()
        {
            var endPoint = new IPEndPoint((IPAddress.Loopback), 54321);
            var manager = new DataManager(new SocketProxy(), endPoint);
            var testingServer = new DirectoryServer(manager, new WebPageMaker(54321), "C:/", new DirectoryProxy(), new FileProxy());
            new Thread(() => runServer(testingServer)).Start();
            
            string rURL = @"http://localhost:54321/C:/Program%20Files%20(x86)/Internet%20Explorer/ie9props.propdesc";
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(rURL);

            wrGETURL.GetResponse();

        }

        public void runServer(IMainServer server)
        {
            server.run();
        }
    }
}
