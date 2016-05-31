using System;

namespace Server.Core
{
    public interface IMainServer
    {
        bool AcceptingNewConn { get; }
        void StopNewConnAndCleanUp();

        void RunningProcess(IZSocket handler, Guid id);

        void Run();
    }
}