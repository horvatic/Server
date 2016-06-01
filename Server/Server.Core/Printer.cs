using System;
using System.IO;

namespace Server.Core
{
    public class Printer : IPrinter
    {
        public string Log { get; set; } = null;

        public void Print(string output)
        {
            lock (this)
            {

                if (Log != null)
                {
                    var ostrm = new FileStream(Log, FileMode.Append, FileAccess.Write);
                    var writer = new StreamWriter(ostrm);
                    Console.SetOut(writer);
                    Console.WriteLine(output);
                    writer.Close();
                    ostrm.Close();

                }
                else
                {
                    Console.WriteLine(output);
                }
            }

        }
    }
}