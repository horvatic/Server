namespace Server.Core
{
    public interface IMainServer
    {
        bool stillAlive();
        void run();
    }
}
