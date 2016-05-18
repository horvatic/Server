using System;
using System.IO;

namespace Server.Core
{
    public class DirectoryProxy : IDirectoryProxy
    {
        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}
