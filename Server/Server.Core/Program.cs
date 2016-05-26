using System;
using System.IO;
using System.Net;
using System.Text;

namespace Server.Core
{
    public class Program
    {
        private const int LowerBoundPort = 2000;
        private const int UpperBoundPort = 65000;
        private const string PortOption = "-p";
        private const string DirectoryOption = "-d";
        public static int Main(string[] args)
        {
            RunServer(MakeServer(args));
            return 0;
        }

        public static void RunServer(IMainServer runningServer)
        {
            var closeServerProcess = new ClosingServerHandler(runningServer);
            Console.CancelKeyPress += closeServerProcess.ShutdownProcess;

            if (runningServer == null) return;
            Console.WriteLine("Server Running...");
            do
            {
                runningServer.Run();
            } while (runningServer.AcceptingNewConn);
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
                        Console.WriteLine(WrongNumberOfArgs());
                        return null;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Another Server is running on that port");
                return null;
            }
        }

        public static IMainServer MakedirectoryServer(string chosenPort, string homeDirectory)
        {
            var cleanHomeDir = homeDirectory.Replace('\\', '/');
            var port = PortWithinRange(chosenPort);
            if (port == -1) return null;
            if (!VaildDrive(cleanHomeDir)) return null;
            var endPoint = new IPEndPoint((IPAddress.Loopback), port);
            var zSocket = new ZSocket(endPoint);
            return new MainServer(zSocket, new WebPageMaker(port), cleanHomeDir, new DirectoryProcessor(),
                new FileProcessor());
        }


        public static IMainServer DirectoryServer(string[] args)
        {
            if (args[0] == PortOption && args[2] == DirectoryOption)
            {
                return MakedirectoryServer(args[1], args[3]);
            }
            if (args[2] == PortOption && args[0] == DirectoryOption)
            {
                return MakedirectoryServer(args[3], args[1]);
            }
            Console.WriteLine(InvaildOption());
            return null;
        }

        public static IMainServer HelloWorldServer(string[] args)
        {
            if (args[0] != PortOption) return null;
            var port = PortWithinRange(args[1]);
            if (port == -1) return null;
            var endPoint = new IPEndPoint((IPAddress.Loopback), port);
            var zSocket = new ZSocket(endPoint);
            return new MainServer(zSocket, new WebPageMaker(), null, new DirectoryProcessor(), new FileProcessor());
        }

        private static bool VaildDrive(string dir)
        {
            if (Directory.Exists(dir))
            {
                return true;
            }
            Console.WriteLine("Not a vaild directory");
            return false;
        }

        private static int PortWithinRange(string port)
        {
            int portconvert;
            if (int.TryParse(port, out portconvert))
            {
                if (portconvert >= LowerBoundPort && portconvert <= UpperBoundPort) return portconvert;
                Console.Write(GetInvaildPortError());
                return -1;
            }
            Console.Write(GetInvaildPortError());
            return -1;
        }

        private static string WrongNumberOfArgs()
        {
            var error = new StringBuilder();
            error.Append("Invaild Number of Arguments.\n");
            error.Append("Can only be -p PORT\n");
            error.Append("or -p PORT -d DIRECTORY\n");
            error.Append("Examples:\n");
            error.Append("Server.exe -p 8080 -d C:/\n");
            error.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            error.Append("Server.exe -p 9999");

            return error.ToString();
        }

        private static string GetInvaildPortError()
        {
            var error = new StringBuilder();
            error.Append("Invaild Port Detected.");
            error.Append("Vaild Ports 2000 - 65000");

            return error.ToString();
        }

        private static string InvaildOption()
        {
            var error = new StringBuilder();
            error.Append("Invaild Input Detected.\n");
            error.Append("Can only be -p\n");
            error.Append("or -p -d\n");
            error.Append("Examples:\n");
            error.Append("Server.exe -p 8080 -d C:/\n");
            error.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            error.Append("Server.exe -p 9999");

            return error.ToString();
        }
    }
}