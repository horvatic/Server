using System.Net;
using System.Text;

namespace Server.Core
{
    public class DataManager : IDataManager
    {
        private readonly ISocketProxy _socket;

        public DataManager(ISocketProxy socket, IPEndPoint localEndPoint)
        {
            _socket = socket;
            socket.Bind(localEndPoint);
            socket.Listen(100);
        }
        
        public DataManager(ISocketProxy socket)
        {
            _socket = socket;
        }

        public bool Connected()
        {
            return _socket.Connected();
        }

        public IDataManager Accept()
        {
            return new DataManager(_socket.Accept());
        }

        public void Close()
        {
            _socket.Close();
        }

        public int Send(string message)
        {
            return _socket.Send(Encoding.ASCII.GetBytes(message));
        }

        public void SendFile(string message)
        {
            _socket.SendFile(message);
        }

        public string Receive()
        {
            var readData = new byte[8192];
            var lengthRead = _socket.Receive(readData);
            return (Encoding.ASCII.GetString(readData).Substring(0, lengthRead));
        }
    }
}