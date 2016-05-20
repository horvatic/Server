namespace Server.Core
{
    public interface IMainServer
    {
        bool StillAlive { get; }
        void RunningProcess(IDataManager handler);

        void Run();
    }
}