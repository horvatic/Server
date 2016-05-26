namespace Server.Core
{
    public interface IZSocket
    {
        bool Connected();
        string Receive();
        int Send(string message);
        void SendFile(string message);
        IZSocket Accept();
        void Close();
    }
}