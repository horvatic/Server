using System;
using System.IO;

namespace Server.Core
{
    public class Printer : IPrinter
    {
        public string Log { get; set; } = null;

        public void Print(string output)
        {
            if (Log != null)
            {
                try
                {
                    var ostrm = new FileStream(Log, FileMode.Append, FileAccess.Write);
                    var writer = new StreamWriter(ostrm);
                    Console.SetOut(writer);
                    Console.WriteLine(output);
                    writer.Close();
                    ostrm.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot open Redirect.txt for writing");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine(output);
            }

        }
    }
}