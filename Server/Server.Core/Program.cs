using System;
using System.Net;

namespace Server.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runningServer = makeServer(args); 
            while (true && runningServer != null)
                runningServer.run();
        }

        public static IMainServer makeServer(string[] args)
        {
            if (args.Length > 1)
            {
                int port;
                if (Int32.TryParse(args[1], out port))
                {
                    var endPoint = new IPEndPoint((IPAddress.Loopback), port);
                    var manager = new DataManager(new SocketProxy(), endPoint);
                    return new HelloWorldServer(manager, new WebPageMaker());
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
}
