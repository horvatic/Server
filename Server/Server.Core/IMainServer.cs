namespace Server.Core
{
    public interface IMainServer
    {
        bool AccectingNewConn { get; }
        void CleanUp();
        void StopNewConn();

        void RunningProcess(IZSocket handler);

        void Run();
    }
}