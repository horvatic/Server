using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public interface IDirectoryProxy
    {
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
    }
}
