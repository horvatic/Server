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
            var httpFactory = new HttpServiceFactory(new MockHttpService());
            Assert.NotNull(httpFactory);
        }

        [Fact]
        public void Make_Mock_Http_Processor_Factory()
        {
            var serverProperties = new ServerProperties("c:/", 5555, 
                new HttpResponse(), new ServerTime(),
                new MockPrinter());
            var httpFactory = new HttpServiceFactory(new MockHttpService());
            var gotMockedHttpService = httpFactory.GetService("GET / HTTP/1.1",
                new List<string>() {"Server.Test"}, 
                new List<Assembly>() { Assembly.GetExecutingAssembly() },
                serverProperties);
            Assert.NotNull(gotMockedHttpService);
        }

        [Fact] public void Http_Processor_Factory_Default()
        {
            var serverProperties = new ServerProperties(null, 
                5555, new HttpResponse(), new ServerTime(),
                new MockPrinter());
            var httpFactory = new HttpServiceFactory(new MockDefaultService());
            var gotMockedHttpService = httpFactory.GetService("GET /Default HTTP/1.1",
                new List<string>() { "Server.Core", "Server.Test" }, 
                new List<Assembly>() { Assembly.GetAssembly(typeof(MockDefaultService)),
                    Assembly.GetExecutingAssembly()}, 
                serverProperties);
            Assert.IsType(typeof(MockDefaultService), gotMockedHttpService);
        }
    }
}