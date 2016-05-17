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
    }
}
