using System.Reflection;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class HttpServiceFactoryTest
    {
        [Fact]
        public void Make_Not_Null_Factory()
        {
            var httpFactory = new HttpServiceFactory(new Service404());
            Assert.NotNull(httpFactory);
        }

        [Fact]
        public void Make_Mock_Http_Processor_Factory()
        {
            var serverProperties = new ServerProperties(null, null, null, 5555, new HttpResponse());
            var httpFactory = new HttpServiceFactory(new Service404());
            var gotMockedHttpService = httpFactory.GetService("GET / HTTP/1.1", Assembly.GetExecutingAssembly(), "Server.Test", serverProperties);
            Assert.NotNull(gotMockedHttpService);
        }
    }
}