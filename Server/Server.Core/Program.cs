using System.Net;

namespace Server.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runningServer = new MainServer(new DataManager(new SocketProxy(), new IPEndPoint((IPAddress.Loopback), 32000)), new WebPageMaker());
            while(true)
                runningServer.run();
        }
    }
}
