using Moq;
using Server.Core;

namespace Server.Test
{
    class MockDefaultService : IHttpServiceProcessor
    {
        private readonly Mock<IHttpServiceProcessor> _mock;

        public MockDefaultService()
        {
            _mock = new Mock<IHttpServiceProcessor>();
        }
        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            return false;
        }

        public IHttpResponse ProcessRequest(string request, 
            IHttpResponse httpResponse, 
            ServerProperties serverProperties)
        {
            return _mock.Object.ProcessRequest(request,
                httpResponse,
                serverProperties);
        }
    }
}
