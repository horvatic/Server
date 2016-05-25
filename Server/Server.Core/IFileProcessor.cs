namespace Server.Core
{
    public interface IFileProcessor
    {
        byte[] ReadAllBytes(string path);
        bool Exists(string path);
    }
}