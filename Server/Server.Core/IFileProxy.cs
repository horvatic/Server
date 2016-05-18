using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public interface IFileProxy
    {
        byte[] ReadAllBytes(string path);
        bool Exists(string path);
    }
}
