using System.Threading;
using Moq;
using Server.Core;

namespace Server.Test
{
    public class MockRequestProcessor : IRequestProcessor
    {
        private readonly Mock<IRequestProcessor> _mock;

        public MockRequestProcessor()
        {
            _mock = new Mock<IRequestProcessor>();
        }

        public string HandleRequest(string request,
            IZSocket handler, ISend sender,
            IHttpServiceProcessor service,
            ServerProperties serverProperties)
        {
            if (request == "GET /throw HTTP/1.1\r\n")
                throw new System.Exception("Error");
            if (request == "GET /Sleep HTTP/1.1\r\n")
                Thread.Sleep(3000);
            return _mock.Object.HandleRequest(request,
            handler, sender,
            service, serverProperties);
        }
    }
}