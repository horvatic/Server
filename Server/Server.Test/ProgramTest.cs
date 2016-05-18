using Server.Core;
using Xunit;
namespace Server.Test
{
    public class ProgramTest
    {
        [Fact]
        public void Make_Server_Correct()
        {
            string[] args = { "-p", "8080", "-d", "C:/Directory" };
            var serverMade = Program.makeServer(args);
            Assert.NotNull(serverMade);
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
            var mockServer = new MockMainServer();
            Program.runServer(mockServer);
            mockServer.VerifyRun();
            mockServer.stillAlive();
        }
    }
}
