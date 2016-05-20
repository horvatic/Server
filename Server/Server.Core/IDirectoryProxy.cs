namespace Server.Core
{
    public interface IDirectoryProxy
    {
        bool Exists(string path);
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
    }
}
