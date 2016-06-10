using System.IO;

namespace Server.Core
{
    public class FileProcessor : IFileProcessor
    {
        public long FileSize(string path)
        {
            return (new FileInfo(path)).Length;
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}