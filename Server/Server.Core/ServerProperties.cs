namespace Server.Core
{
    public class ServerProperties
    {
        public ServerProperties(string currentDir, IDirectoryProcessor dirReader, IFileProcessor fileReader, int port,
            IHttpResponse defaultResponse, IServerTime time, IPrinter io)
        {
            if (currentDir == null)
                CurrentDir = null;
            else
                CurrentDir = currentDir.EndsWith("/") ? currentDir : currentDir + "/";
            DirReader = dirReader;
            FileReader = fileReader;
            Port = port;
            DefaultResponse = defaultResponse;
            Time = time;
            Io = io;
        }

        public string CurrentDir { get; }
        public IDirectoryProcessor DirReader { get; }
        public IFileProcessor FileReader { get; }
        public int Port { get; }
        public IHttpResponse DefaultResponse { get; }

        public IServerTime Time { get; }

        public IPrinter Io { get; }
    }
}