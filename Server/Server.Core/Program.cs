using System;
using System.IO;
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
            if (args.Length == 2)
            {
                return helloWorldServer(args);
            }
            else if(args.Length == 4)
            {
                return directoryServerServer(args);
            }
            else
                return null;
        }

        public static IMainServer directoryServerServer(string[] args)
        {
            int port;
            string homeDirectory;
            if (args[0] == "-p" && args[2] == "-d")
            {
                homeDirectory = args[3];
                if (Int32.TryParse(args[1], out port) && Directory.Exists(homeDirectory))
                {
                    var endPoint = new IPEndPoint((IPAddress.Loopback), port);
                    var manager = new DataManager(new SocketProxy(), endPoint);
                    return new DirectoryServer(manager, new WebPageMaker(new DirectoryProxy(), port), homeDirectory);
                }
                else
                    return null;
            }
            else
                return null;
        }

        public static IMainServer helloWorldServer(string[] args)
        {
            int port;
            if (args[0] == "-p")
            {
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
