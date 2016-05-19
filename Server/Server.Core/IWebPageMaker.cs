namespace Server.Core
{
    public interface IWebPageMaker
    {
        string error404Page();
        string helloWorld();
        string directoryContents(string dir, IDirectoryProxy reader);
    }
}
