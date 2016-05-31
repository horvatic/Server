namespace Server.Core
{
    public class ServerProperties
    {
        public string CurrentDir { get; }
        public IDirectoryProcessor DirReader { get; }
        public IFileProcessor FileReader { get; }
        public int Port { get; }
        public IHttpResponse DefaultResponse { get; }

        public ServerProperties(string currentDir, IDirectoryProcessor dirReader, IFileProcessor fileReader, int port, IHttpResponse defaultResponse)
        {
            if (currentDir == null)
                CurrentDir = null;
            else
                CurrentDir = currentDir.EndsWith("/") ? currentDir : currentDir + "/";
            DirReader = dirReader;
            FileReader = fileReader;
            Port = port;
            DefaultResponse = defaultResponse;
        }


    }
}
