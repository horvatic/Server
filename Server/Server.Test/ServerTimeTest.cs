using Server.Core;
using Xunit;

namespace Server.Test
{
    public class ServerTimeTest
    {
        [Fact]
        public void Make_Not_Null_Class()
        {
            Assert.NotNull(new ServerTime());
        }

        [Fact]
        public void Get_Time_On_Server()
        {
            Assert.NotEqual("", new ServerTime().GetTime());
        }
    }
}