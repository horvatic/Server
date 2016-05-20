using System;
using System.IO;
using System.Net;
using System.Text;

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
                Console.WriteLine("Server Running...");
                do
                {
                    runningServer.run();
                } while (runningServer.stillAlive());
            }
            else
                Console.WriteLine(getError());
        }

        public static string getError()
        {
            var error = new StringBuilder();
            error.Append("Invaild Input Detected.\n");
            error.Append("Server.exe may already be running on port\n");
            error.Append("must be Server.Core.exe -p PORT -d directory\n");
            error.Append("Vaild Ports 2000 - 65000\n");
            error.Append("Examples:\n");
            error.Append("Server.exe -p 8080 -d C:/\n");
            error.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            error.Append("Server.exe -p 9999\n\r\n");

            return error.ToString();
        }
        public static IMainServer makeServer(string[] args)
        {
            try
            {
                if (args.Length == 2)
                {
                    return helloWorldServer(args);
                }
                else if (args.Length == 4)
                {
                    return directoryServer(args);
                }
                else
                    return null;
            }
            catch(Exception e)
            {
                return null;
            }
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
                    if (port > 1999 && port < 65001)
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
            else
                return null;
        }
    }
}
