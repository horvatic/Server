namespace Server.Core
{
    public interface IMainServer
    {
        void StopNewConn();
        bool StillAlive { get; }

        void RunningProcess(IDataManager handler);

        void Run();
    }
}