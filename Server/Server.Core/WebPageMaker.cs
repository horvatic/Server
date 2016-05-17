using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class WebPageMaker : IWebPageMaker
    {
        public string helloWorld()
        {
            return "<h1>Hello World</h1>";
        }
    }
}
