namespace Server.Core
{
    public interface IMainServer
    {
        void runningProcess(IDataManager handler);
        bool stillAlive();
        void run();
    }
}
