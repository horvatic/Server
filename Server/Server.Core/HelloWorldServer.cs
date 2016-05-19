using System.Text;

namespace Server.Core
{
    public class HelloWorldServer : IMainServer
    {
        IDataManager socket;
        IWebPageMaker webMaker;
        public HelloWorldServer(IDataManager socket, IWebPageMaker webMaker)
        {
            this.socket = socket;
            this.webMaker = webMaker;
        }
        public void run()
        {
            var handler = socket.accept();
            var request = handler.receive();
            if (!(request.Length == 0))
            {
                if (request.Contains("GET / HTTP/1.1"))
                { 
                    handler.send("HTTP/1.1 200 OK\r\n");
                    handler.send("Content-Type: text/html\r\n");
                    handler.send("Content-Length: " + Encoding.ASCII.GetBytes(webMaker.helloWorld()).Length + "\r\n\r\n");
                    handler.send(webMaker.helloWorld());
                }
                else
                {
                    handler.send("HTTP/1.1 404 Not Found\r\n");
                    handler.send("Content-Type: text/html\r\n");
                    handler.send("Content-Length: " + Encoding.ASCII.GetBytes("404").Length + "\r\n\r\n");
                    handler.send("404");
                }
            }
            handler.close();
        }

        public bool stillAlive()
        {
            return true;
        }
    }
}

/*
            string[] files = Directory.GetFiles(@"C:\ShanwDocs\Apprenticeships\week5");
            StringBuilder str = new StringBuilder();
            foreach (string file in files)
            {
                str.Append("<br><a href=http://localhost:32000/" + file.Replace('\\', '/')+ " download>" + file + "</a>");
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
                        handler.Send(Encoding.ASCII.GetBytes("HTTP/1.1 404 Bad Request\r\n"));
                        Console.WriteLine("Write One");
                        handler.Send(Encoding.ASCII.GetBytes("Content-Length: " + 0 + "\r\n\r\n"));
                        Console.WriteLine("Write Two");
                    }
                    else if(requestInformation.Contains("/C:/ShanwDocs/Apprenticeships/week5/Task.txt"))
                    {
                        Console.WriteLine(requestInformation);
                        Console.WriteLine("Read Data");
                        handler.Send(Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n"));
                        Console.WriteLine("Write One");
                        handler.Send(Encoding.ASCII.GetBytes("Content-Type: application/download\r\n"));
                        Console.WriteLine("Write Two");
                        handler.Send(Encoding.ASCII.GetBytes("Content-Length: " + System.IO.File.ReadAllBytes("C:/ShanwDocs/Apprenticeships/week5/Task.txt").Length + "\r\n\r\n"));
                        Console.WriteLine("Write Three");
                        handler.SendFile("C:/ShanwDocs/Apprenticeships/week5/Task.txt");
                        Console.WriteLine("Write Four");
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
            */
