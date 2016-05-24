namespace Server.Core
{
    public interface IMainServer
    {
        bool StillAlive { get; }
        void StopNewConn();

        void RunningProcess(IDataManager handler);

        void Run();
    }
}