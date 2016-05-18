using System;
using System.Net;

namespace Server.Core
{
    public class Program
    {
        public static int Main(string[] args)
        {
            runServer(makeServer(args));
            return 0;
        }

        public static void runServer(IMainServer runningServer)
        {
            if (runningServer != null)
            {
                do
                {
                    runningServer.run();
                } while (runningServer.stillAlive());
            }
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
