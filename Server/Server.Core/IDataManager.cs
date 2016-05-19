namespace Server.Core
{
    public interface IDataManager
    {
        string receive();
        int send(string message);
        void sendFile(string message);
        IDataManager accept();
        void close();
    }
}
