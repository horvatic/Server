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
                return directoryServer(args);
            }
            else
                return null;
        }

        public static IMainServer makedirectoryServer(string chosenPort, string homeDirectory)
        {
            int port;
            if (Int32.TryParse(chosenPort, out port) && Directory.Exists(homeDirectory))
            {
                if (port > 1999 && port < 65001)
                {
                    var endPoint = new IPEndPoint((IPAddress.Loopback), port);
                    var manager = new DataManager(new SocketProxy(), endPoint);
                    return new DirectoryServer(manager, new WebPageMaker(port), homeDirectory, new DirectoryProxy(), new FileProxy());
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public static IMainServer directoryServer(string[] args)
        {
            if (args[0] == "-p" && args[2] == "-d")
            {
                return makedirectoryServer(args[1], args[3]);
            }
            else if (args[2] == "-p" && args[0] == "-d")
            {
                return makedirectoryServer(args[3], args[1]);
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
