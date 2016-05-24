using System;
using System.IO;
using System.Net;
using System.Text;

namespace Server.Core
{
    public class Program
    {
        public delegate void ShutDown(object sender, ConsoleCancelEventArgs e);

        public static bool StopServer;

        public static void ShutDownServer(object sender, ConsoleCancelEventArgs e)
        {
            StopServer = true;
        }

        public static int Main(string[] args)
        {
            RunServer(MakeServer(args));
            return 0;
        }

        public static void RunServer(IMainServer runningServer)
        {
            Console.CancelKeyPress += ShutDownServer;

            if (runningServer != null)
            {
                Console.WriteLine("Server Running...");
                do
                {
                    runningServer.Run();
                    if (!StopServer) continue;
                    runningServer.StopNewConn();
                    Console.WriteLine("Server Shuting Down...");
                } while (runningServer.StillAlive);
            }
            else
                Console.WriteLine(GetError());
        }

        public static string GetError()
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

        public static IMainServer MakeServer(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 2:
                        return HelloWorldServer(args);
                    case 4:
                        return DirectoryServer(args);
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IMainServer MakedirectoryServer(string chosenPort, string homeDirectory)
        {
            int port;
            if (int.TryParse(chosenPort, out port) && Directory.Exists(homeDirectory))
            {
                if (port > 1999 && port < 65001)
                {
                    var endPoint = new IPEndPoint((IPAddress.Loopback), port);
                    var manager = new DataManager(new SocketProxy(), endPoint);
                    return new MainServer(manager, new WebPageMaker(port), homeDirectory, new DirectoryProxy(),
                        new FileProxy());
                }
                return null;
            }
            return null;
        }

        public static IMainServer DirectoryServer(string[] args)
        {
            if (args[0] == "-p" && args[2] == "-d")
            {
                return MakedirectoryServer(args[1], args[3]);
            }
            if (args[2] == "-p" && args[0] == "-d")
            {
                return MakedirectoryServer(args[3], args[1]);
            }
            return null;
        }

        public static IMainServer HelloWorldServer(string[] args)
        {
            if (args[0] == "-p")
            {
                int port;
                if (int.TryParse(args[1], out port))
                {
                    if (port > 1999 && port < 65001)
                    {
                        var endPoint = new IPEndPoint((IPAddress.Loopback), port);
                        var manager = new DataManager(new SocketProxy(), endPoint);
                        return new MainServer(manager, new WebPageMaker(), null, new DirectoryProxy(), new FileProxy());
                    }
                    return null;
                }
                return null;
            }
            return null;
        }
    }
}