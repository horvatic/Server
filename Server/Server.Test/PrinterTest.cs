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
            var gid = Guid.NewGuid();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            var io = new Printer {Log = "c:/" + gid + ".txt"};
            io.Print("Hello");
            Assert.True(File.Exists("c:/" + gid + ".txt"));
            File.Delete("c:/" + gid + ".txt");
        }

        [Fact]
        public void Print_To_File()
        {
            var gid = Guid.NewGuid();
            var io = new Printer();
            io.PrintToFile("Hello", "c:/" + gid + ".txt");
            Assert.True(File.Exists("c:/" + gid + ".txt"));
            File.Delete("c:/" + gid + ".txt");
        }
    }
}