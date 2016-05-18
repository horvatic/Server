using Server.Core;
using Xunit;
namespace Server.Test
{
    public class ProgramTest
    {
        [Fact]
        public void Make_Dirctory_Server_Correct()
        {
            string[] args = { "-p", "32000", "-d", "C:\\" };
            var serverMade = Program.makeServer(args);
            Assert.NotNull(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Correct_Inccorect_Arg_BAckwords()
        {
            string[] args = { "-d", "C:\\", "-p", "8080" };
            var serverMade = Program.makeServer(args);
            Assert.NotNull(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct()
        {
            string[] args = { "-p", "32000", "-d", "-d" };
            var serverMade = Program.makeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Dirctory_Server_Inncorect_Correct_3_Settings()
        {
            string[] args = { "-p", "32000", "-d" };
            var serverMade = Program.makeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Hello_World_Server_Correct()
        {
            string[] args = { "-p", "8080"};
            var serverMade = Program.makeServer(args);
            Assert.NotNull(serverMade);
        }

        [Fact]
        public void Make_Hello_World_Incorrect_Correct()
        {
            string[] args = { "8080", "-p" };
            var serverMade = Program.makeServer(args);
            Assert.Null(serverMade);
        }
        [Fact]
        public void Make_Hello_World_Incorrect_Correct_No_Port()
        {
            string[] args = { "-p", "-p" };
            var serverMade = Program.makeServer(args);
            Assert.Null(serverMade);
        }
        [Fact]
        public void Make_Server_Inncorect_NoArgs()
        {
            var args = new string[1];
            var serverMade = Program.makeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Make_Server_Inncorect_Wrong_Args()
        {
            string[] args = { "-p", "-b", "-d", "C:/Directory" };
            var serverMade = Program.makeServer(args);
            Assert.Null(serverMade);
        }

        [Fact]
        public void Main_Starting_Program()
        {
            string[] args = { };
            Assert.Equal(0, Program.Main(args));
        }

        [Fact]
        public void Test_Running_Of_Server()
        {
            var mockServer = new MockMainServer().stubStillAlive();
            Program.runServer(mockServer);
            mockServer.VerifyRun();
            mockServer.VerifyStillAlive();
        }
    }
}
