namespace Server.Core
{
    public class ServerProperties
    {
        public ServerProperties(string currentDir, 
            int port,
            IHttpResponse defaultResponse, 
            IServerTime time,
            IPrinter io,
            object persistentData = null)
        {
            if (currentDir == null)
                CurrentDir = null;
            else
                CurrentDir = currentDir.EndsWith("/") ? currentDir : currentDir + "/";
            Port = port;
            DefaultResponse = defaultResponse;
            Time = time;
            Io = io;
            PersistentData = persistentData;
        }

        public string CurrentDir { get; }
        public int Port { get; }
        public IHttpResponse DefaultResponse { get; }

        public IServerTime Time { get; }

        public IPrinter Io { get; }

        public object PersistentData { get; set; }
    }
}