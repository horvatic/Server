using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.TcpClient;
using System.Text;

namespace Server.Core
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] files = Directory.GetFiles(@"C:\ShanwDocs\Apprenticeships\week5");
            StringBuilder str = new StringBuilder();
            foreach (string file in files)
            {
                str.Append("<br><a href=http://localhost:32000/" + file.Replace('\\', '/')+">" + file + "</a>");
            }
            
            string strOutput = str.ToString();

            IPEndPoint localEndPoint = new IPEndPoint((IPAddress.Loopback), 32000);
            Socket listener = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);


            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                Socket handler = listener.Accept();
                
                byte[] readData = new byte[8192];
                Console.WriteLine("Hit");
                int lengthRead = handler.Receive(readData);
                if (!(lengthRead == 0))
                {
                    string requestInformation = Encoding.ASCII.GetString(readData).Substring(0, lengthRead);
                    if (requestInformation.Contains("favicon.ico"))
                    {
                        Console.WriteLine(requestInformation);
                        Console.WriteLine("Read Data");
                        handler.Send(Encoding.ASCII.GetBytes("HTTP/1.1 400 Bad Request\r\n"));
                        Console.WriteLine("Write One");
                        handler.Send(Encoding.ASCII.GetBytes("Content-Length: " + 0 + "\r\n\r\n"));
                        Console.WriteLine("Write Two");
                    }
                    else
                    {
                        Console.WriteLine(requestInformation);
                        Console.WriteLine("Read Data");
                        handler.Send(Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n"));
                        Console.WriteLine("Write One");
                        handler.Send(Encoding.ASCII.GetBytes("Content-Type: text/html\r\n"));
                        Console.WriteLine("Write Two");
                        handler.Send(Encoding.ASCII.GetBytes("Content-Length: " + Encoding.ASCII.GetBytes(strOutput).Length + "\r\n\r\n"));
                        Console.WriteLine("Write Three");
                        handler.Send(Encoding.ASCII.GetBytes(strOutput));
                        Console.WriteLine("Write Four");
                    }
                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}

/*
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.
                Socket handler = listener.Accept();
                byte[] readData = new byte[1024];
                Console.WriteLine("Hit");
                handler.Send(Encoding.ASCII.GetBytes(strOutput));
                Console.WriteLine(handler.Receive(readData));
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                handler = listener.Accept();
                handler.Send(Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n"));
                Console.WriteLine(handler.Receive(readData));
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                handler = listener.Accept();
                handler.Send(Encoding.ASCII.GetBytes("Connection: close\r\n\r\n"));
                //handler.SendFile("C:\\ShanwDocs\\Apprenticeships\\week5\\webData\\HelloWorld.html");
                //handler.Send(System.IO.File.ReadAllBytes("C:\\ShanwDocs\\Apprenticeships\\week5\\webData\\HelloWorld.html"));
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                */
