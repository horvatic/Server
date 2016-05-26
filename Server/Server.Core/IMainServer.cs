namespace Server.Core
{
    public interface IMainServer
    {
        bool AcceptingNewConn { get; }
        void CleanUp();
        void StopNewConn();

        void RunningProcess(IZSocket handler);

        void Run();
    }
}