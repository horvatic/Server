namespace Server.Core
{
    public interface IMainServer
    {
        bool AcceptingNewConn { get; }
        void StopNewConnAndCleanUp();

        void RunningProcess(object poolIdAndSocket);

        void Run();
    }
}