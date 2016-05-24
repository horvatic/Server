
namespace Server.Core
{
    public interface IDataManager
    {
        bool Connected();
        string Receive();
        int Send(string message);
        void SendFile(string message);
        IDataManager Accept();
        void Close();
    }
}