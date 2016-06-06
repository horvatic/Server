using System;
using System.Collections.Generic;
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
            var serverProperties = new ServerProperties(null, null, null, 5555, new HttpResponse(), new ServerTime(),
                new MockPrinter());
            var httpFactory = new HttpServiceFactory(new Service404());
            var gotMockedHttpService = httpFactory.GetService("GET / HTTP/1.1", Assembly.GetExecutingAssembly(),
                new List<string>() {"Server.Test"}, serverProperties);
            Assert.NotNull(gotMockedHttpService);
        }

        [Fact]
        public void Http_Processor_Factory_Upload()
        {
            var serverProperties = new ServerProperties(null, null, null, 5555, new HttpResponse(), new ServerTime(),
                new MockPrinter());
            var httpFactory = new HttpServiceFactory(new Service404());
            var gotMockedHttpService = httpFactory.GetService("GET /upload HTTP/1.1", Assembly.GetAssembly(typeof(Ftpservice)),
                new List<string>() { "Server.Core", "Server.Test" }, serverProperties);
            Assert.IsType(typeof(Ftpservice), gotMockedHttpService);
        }
    }
}