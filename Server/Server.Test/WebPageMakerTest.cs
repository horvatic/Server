using Xunit;
using Server.Core;
namespace Server.Test
{
    public class WebPageMakerTest
    {
        [Fact]
        public void Making_Hello_World_Page()
        {
            WebPageMaker maker = new WebPageMaker();
            Assert.Equal("<h1>Hello World</h1>", maker.helloWorld());
        }
    }
}
