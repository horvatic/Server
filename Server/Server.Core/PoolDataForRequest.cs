using System;

namespace Server.Core
{
    public class PoolDataForRequest
    {
        public PoolDataForRequest(IZSocket handler, Guid id)
        {
            Handler = handler;
            Id = id;
        }

        public IZSocket Handler { get; }
        public Guid Id { get; }
    }
}