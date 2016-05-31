using System;

namespace Server.Core
{
    public class ClosingServerHandler
    {
        public delegate void Shutdown(object sender, ConsoleCancelEventArgs e);

        private readonly IMainServer _runningServer;

        public ClosingServerHandler(IMainServer runningServer)
        {
            _runningServer = runningServer;
        }

        public void ShutdownProcess(object sender, ConsoleCancelEventArgs e)
        {
            //Console.WriteLine("Server Shuting Down...");
            _runningServer.StopNewConnAndCleanUp();
        }
    }
}