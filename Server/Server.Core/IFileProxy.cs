namespace Server.Core
{
    public interface IFileProxy
    {
        byte[] ReadAllBytes(string path);
        bool Exists(string path);
    }
}