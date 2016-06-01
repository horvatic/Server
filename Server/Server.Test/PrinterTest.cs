using System;
using System.IO;
using Server.Core;
using Xunit;
namespace Server.Test
{
    public class PrinterTest
    {
        [Fact]
        public void Make_Not_Null_Class()
        {
            Assert.NotNull(new Printer());
        }

        [Fact]
        public void Output_Given_Input()
        {
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var io = new Printer();
            io.Print("Hello");
            Assert.Equal("Hello\r\n", consoleOutput.ToString());
        }

        [Fact]
        public void Output_Given_Input_Log()
        {
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            var io = new Printer {Log = "c:/testFile.txt"};
            io.Print("Hello");
        }
    }
}
