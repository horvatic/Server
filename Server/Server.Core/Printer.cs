using System;
using System.IO;
using System.Text;

namespace Server.Core
{
    public class Printer : IPrinter
    {
        public string Log { get; set; } = null;

        public void PrintToFile(string output, string path)
        {
            var write = new FileStream(path, FileMode.Append, FileAccess.Write);
            var outputConverted = Encoding.ASCII.GetBytes(output);
            write.Write(outputConverted, 0 , outputConverted.Length);
            write.Close();
        }
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