using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public interface IDataManager
    {
        string receive();
        int send(string message);
        void sendFile(string message);
        IDataManager accept();
        void close();
    }
}
