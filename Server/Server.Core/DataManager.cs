using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class DataManager : IDataManager
    {
        private ISocketProxy socket;
        public DataManager(ISocketProxy socket, IPEndPoint localEndPoint)
        {
            this.socket = socket;
            socket.Bind(localEndPoint);
            socket.Listen(10);
        }
        public DataManager(ISocketProxy socket)
        {
            this.socket = socket;
        }
        public IDataManager accept()
        {

            return new DataManager(socket.Accept());
        }

        public void close()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public int send(string message)
        {
            return socket.Send(Encoding.ASCII.GetBytes(message));
        }

        public void sendFile(string message)
        {
            socket.SendFile(message);
        }

        public string receive()
        {
            byte[] readData = new byte[8192];
            int lengthRead = socket.Receive(readData);
            return (Encoding.ASCII.GetString(readData).Substring(0, lengthRead));
        }
    }
}
