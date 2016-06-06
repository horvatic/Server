using System;

namespace Server.Core
{
    public class ServerTime : IServerTime
    {
        public string GetTime()
        {
            return DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ssZ");
        }
    }
}