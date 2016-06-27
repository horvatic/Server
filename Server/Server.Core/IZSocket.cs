namespace Server.Core
{
    public interface IZSocket
    {
        bool Connected();
        string Receive();
        int Send(byte[] packet, int size);
        IZSocket Accept();
        void Close();
    }
}