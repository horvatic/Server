using System;

namespace Server.Core
{
    public class ClosingServerHandler
    {
        private readonly IMainServer _runningServer;
        public delegate void Shutdown(object sender, ConsoleCancelEventArgs e);
        public ClosingServerHandler(IMainServer runningServer)
        {
            _runningServer = runningServer;
        }

        public void ShutdownProcess(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Server Shuting Down...");
            _runningServer.StopNewConn();
            _runningServer.CleanUp();
        }
    }
}
