namespace Server.Core
{
    public interface IFileProcessor
    {
        long FileSize(string path);
        //byte[] ReadAllBytes(string path);
        bool Exists(string path);
    }
}