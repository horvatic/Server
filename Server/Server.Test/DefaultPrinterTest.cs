using System;
using System.IO;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class DefaultPrinterTest
    {
        [Fact]
        public void Make_Not_Null_Class()
        {
            Assert.NotNull(new DefaultPrinter());
        }

        [Fact]
        public void Output_Given_Input()
        {
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var io = new DefaultPrinter();
            io.Print("Hello");
            Assert.Equal("Hello\r\n", consoleOutput.ToString());
        }

        [Fact]
        public void Output_Given_Input_Log()
        {
            var gid = Guid.NewGuid();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            var io = new DefaultPrinter
                { Log = "c:/" + gid + ".txt"};
            io.Print("Hello");
            Assert.True(File.Exists("c:/" + gid + ".txt"));
            File.Delete("c:/" + gid + ".txt");
        }

        [Fact]
        public void Print_To_File()
        {
            var gid = Guid.NewGuid();
            var io = new DefaultPrinter();
            io.PrintToFile("Hello", "c:/" + gid + ".txt");
            Assert.True(File.Exists("c:/" + gid + ".txt"));
            File.Delete("c:/" + gid + ".txt");
        }
    }
}