using System;
using System.IO;
using System.Text;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class ProgramTest
    {
        public void User_Presses_CTRL_C()
        {
            var dataManager = new MockDataManager();
            var webMaker = new WebPageMaker();
            dataManager = dataManager.StubAccpetObject(dataManager);
            var server = new MainServer(dataManager, webMaker, null, new MockDirectoryProxy(),
                new MockFileProxy());

            var output = new StringWriter();
            Console.SetOut(output);
            var mockServer = new MockMainServer().StubAccectingNewConn();

            var testingThead = new Thread(() => Program.RunServer(server));
            testingThead.Start();
            //Program.ShutDownServer(null, null);
            testingThead.Join();
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        [InlineData(1999)]
        [InlineData(65001)]
        [InlineData(9999999)]
        public void Out_Of_Range_Ports(int invaildPorts)
        {
            var firstOutput = new StringWriter();
            Console.SetOut(firstOutput);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Port Detected.");
            correctOutput.Append("Vaild Ports 2000 - 65000");

            string[] argsHelloWorld = {"-p", invaildPorts.ToString()};
            var serverMadeHelloWorld = Program.MakeServer(argsHelloWorld);
            Assert.Null(serverMadeHelloWorld);
            Assert.Equal(correctOutput.ToString(), firstOutput.ToString());
            firstOutput.Close();

            var secondOutput = new StringWriter();
            Console.SetOut(secondOutput);
            string[] args = {"-p", invaildPorts.ToString(), "-d", "C:\\"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
            Assert.Equal(correctOutput.ToString(), secondOutput.ToString());
            secondOutput.Close();

            var thridOutput = new StringWriter();
            Console.SetOut(thridOutput);
            string[] argsSwaped = {"-d", "C:\\", "-p", invaildPorts.ToString()};
            var serverMadeSwaped = Program.MakeServer(argsSwaped);
            Assert.Null(serverMadeSwaped);
            Assert.Equal(correctOutput.ToString(), thridOutput.ToString());
            thridOutput.Close();
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
            var firstOutput = new StringWriter();
            Console.SetOut(firstOutput);
            var correctOutput = new StringBuilder();

            string[] args = {"-p", "8765", "-d", "C:\\"};
            var serverMade = Program.MakeServer(args);
            Assert.NotNull(serverMade);

            var serverMadeInvaild = Program.MakeServer(args);
            Assert.Null(serverMadeInvaild);
            Assert.Equal("Another Server is running on that port\r\n", firstOutput.ToString());
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
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();

            string[] args = {"-d", "Hello", "-p", "3258"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);

            Assert.Equal("Not a vaild directory\r\n", output.ToString());
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Port()
        {
            var firstOutput = new StringWriter();
            Console.SetOut(firstOutput);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Port Detected.");
            correctOutput.Append("Vaild Ports 2000 - 65000");

            string[] args = {"-d", "C:\\", "-p", "hello"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
            Assert.Equal(correctOutput.ToString(), firstOutput.ToString());
            firstOutput.Close();
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Dir_Option()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Input Detected.\n");
            correctOutput.Append("Can only be -p\n");
            correctOutput.Append("or -p -d\n");
            correctOutput.Append("Examples:\n");
            correctOutput.Append("Server.exe -p 8080 -d C:/\n");
            correctOutput.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            correctOutput.Append("Server.exe -p 9999\r\n");

            string[] args = {"-s", "C:\\", "-p", "9999"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
            Assert.Equal(correctOutput.ToString(), output.ToString());
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_Not_Port_Option()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Input Detected.\n");
            correctOutput.Append("Can only be -p\n");
            correctOutput.Append("or -p -d\n");
            correctOutput.Append("Examples:\n");
            correctOutput.Append("Server.exe -p 8080 -d C:/\n");
            correctOutput.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            correctOutput.Append("Server.exe -p 9999\r\n");

            string[] args = {"-d", "C:\\", "-s", "9999"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
            Assert.Equal(correctOutput.ToString(), output.ToString());
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();

            string[] args = {"-p", "32000", "-d", "-d"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);

            Assert.Equal("Not a vaild directory\r\n", output.ToString());
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_3_Settings()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Number of Arguments.\n");
            correctOutput.Append("Can only be -p PORT\n");
            correctOutput.Append("or -p PORT -d DIRECTORY\n");
            correctOutput.Append("Examples:\n");
            correctOutput.Append("Server.exe -p 8080 -d C:/\n");
            correctOutput.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            correctOutput.Append("Server.exe -p 9999\r\n");


            string[] args = {"-p", "32000", "-d"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
            Assert.Equal(correctOutput.ToString(), output.ToString());
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
            var output = new StringWriter();
            Console.SetOut(output);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Number of Arguments.\n");
            correctOutput.Append("Can only be -p PORT\n");
            correctOutput.Append("or -p PORT -d DIRECTORY\n");
            correctOutput.Append("Examples:\n");
            correctOutput.Append("Server.exe -p 8080 -d C:/\n");
            correctOutput.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            correctOutput.Append("Server.exe -p 9999\r\n");

            var args = new string[1];
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);
            Assert.Equal(correctOutput.ToString(), output.ToString());
        }

        [Fact]
        public void Make_Server_Inncorect_Wrong_Args()
        {
            var firstOutput = new StringWriter();
            Console.SetOut(firstOutput);
            var correctOutput = new StringBuilder();
            correctOutput.Append("Invaild Port Detected.");
            correctOutput.Append("Vaild Ports 2000 - 65000");

            string[] args = {"-p", "-b", "-d", "C:/Directory"};
            var serverMade = Program.MakeServer(args);
            Assert.Null(serverMade);

            Assert.Equal(correctOutput.ToString(), firstOutput.ToString());
            firstOutput.Close();
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
            var mockServer = new MockMainServer().StubAccectingNewConn();
            Program.RunServer(mockServer);
            mockServer.VerifyRun();
            mockServer.VerifyAccectingNewConn();
            Assert.Equal("Server Running...\r\n", output.ToString());
        }

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