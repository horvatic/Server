using System;
using System.IO;
using System.Text;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class ProgramTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        [InlineData(1999)]
        [InlineData(65001)]
        [InlineData(9999999)]
        public void Out_Of_Range_Ports(int invaildPorts)
        {
            string[] argsHelloWorld = {"-p", invaildPorts.ToString()};
            var serverMadeHelloWorld = Program.MakeServer(argsHelloWorld);
            Assert.Null(serverMadeHelloWorld);

            string[] args = {"-p", invaildPorts.ToString(), "-d", "C:\\"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);

            string[] argsSwaped = {"-d", "C:\\", "-p", invaildPorts.ToString()};
            var serverMadeSwaped = Program.MakeServer(argsSwaped);
            Assert.Null(serverMadeSwaped);
        }

        [Fact]
        public void Make_Dirctory_Server_Correct()
        {
            string[] args = {"-p", "32000", "-d", "C:\\"};
            var serverMade = Program.MakeServer(args);
            Assert.NotNull(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Twice_Same_Port()
        {
            string[] args = {"-p", "8765", "-d", "C:\\"};
            var serverMade = Program.MakeServer(args);
            Assert.NotNull(serverMade);

            var serverMadeInvaild = Program.MakeServer(args);
            Assert.Null(serverMadeInvaild);
        }

        [Fact]
        public void Make_Dirctory_Server_Correct_Arg_Backwords()
        {
            string[] args = {"-d", "C:\\", "-p", "2020"};
            var serverMade = Program.MakeServer(args);
            Assert.NotNull(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Dir()
        {
            string[] args = {"-d", "Hello", "-p", "1111"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Port()
        {
            string[] args = {"-d", "C:\\", "-p", "hello"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Dir_Option()
        {
            string[] args = {"-s", "C:\\", "-p", "hello"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Port_Option()
        {
            string[] args = {"-d", "C:\\", "-s", "hello"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct()
        {
            string[] args = {"-p", "32000", "-d", "-d"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_3_Settings()
        {
            string[] args = {"-p", "32000", "-d"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Hello_World_Server_Correct()
        {
            string[] args = {"-p", "9560"};
            var serverMade = Program.MakeServer(args);
            Assert.NotNull(serverMade);
        }

        [Fact]
        public void Make_Hello_World_Incorrect_Correct()
        {
            string[] args = {"2750", "-p"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Hello_World_Incorrect_Correct_No_Port()
        {
            string[] args = {"-p", "-p"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Server_Inncorect_NoArgs()
        {
            var args = new string[1];
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Server_Inncorect_Wrong_Args()
        {
            string[] args = {"-p", "-b", "-d", "C:/Directory"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Main_Starting_Program()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            string[] args = {};
            Assert.Equal(0, Program.Main(args));
        }

        [Fact]
        public void Test_Running_Of_Server()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var mockServer = new MockMainServer().StubStillAlive();
            Program.RunServer(mockServer);
            mockServer.VerifyRun();
            mockServer.VerifyStillAlive();
            Assert.Equal("Server Running...\r\n", output.ToString());
        }

        [Fact]
        public void Test_User_Inputs_Invaild_Settings_Dump_Error()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Input Detected.\n");
            correctOutput.Append("Server.exe may already be running on port\n");
            correctOutput.Append("must be Server.Core.exe -p PORT -d directory\n");
            correctOutput.Append("Vaild Ports 2000 - 65000\n");
            correctOutput.Append("Examples:\n");
            correctOutput.Append("Server.exe -p 8080 -d C:/\n");
            correctOutput.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            correctOutput.Append("Server.exe -p 9999\n\r\n\r\n");

            Program.RunServer(null);
            Assert.Equal(correctOutput.ToString(), output.ToString());
        }
    }
}