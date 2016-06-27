using System;

namespace Server.Core
{
    public class PoolDataForRequest
    {
        public PoolDataForRequest(HttpResponse response,
            IZSocket handler, Guid id)
        {
            Handler = handler;
            Id = id;
            Response = response;
        }

        public IZSocket Handler { get; }
        public Guid Id { get; }
        public HttpResponse Response { get; }
    }
}