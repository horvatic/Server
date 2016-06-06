namespace Server.Core
{
    public interface IPrinter
    {
        string Log { get; set; }
        void Print(string output);

        void PrintToFile(string output, string path);
    }
}