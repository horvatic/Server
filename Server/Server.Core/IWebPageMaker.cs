namespace Server.Core
{
    public interface IWebPageMaker
    {
        string Error404Page();
        string HelloWorld();
        string DirectoryContents(string dir, IDirectoryProxy reader, string root);
    }
}