namespace Server.Core
{
    public interface IWebPageMaker
    {
        string helloWorld();
        string directoryContents(string dir);
    }
}
