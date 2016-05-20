namespace Server.Core
{
    public interface IDataManager
    {
        bool connected();
        string receive();
        int send(string message);
        void sendFile(string message);
        IDataManager accept();
        void close();
    }
}
