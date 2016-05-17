using System;
using Server.Core;

namespace Server.Test
{
    class MockDataManager : IDataManager
    {
        public IDataManager accept()
        {
            return this;
        }

        public void close()
        {
            
        }

        public string receive()
        {
            throw new NotImplementedException();
        }

        public int send(string message)
        {
            return message.Length;
        }

        public void sendFile(string message)
        {
           
        }
    }
}
