using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public interface IPrinter
    {
        string Log { get; set; }
        void Print(string output);
    }
}
