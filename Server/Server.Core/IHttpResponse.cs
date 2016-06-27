using System.Collections.Generic;

namespace Server.Core
{
    public interface IHttpResponse
    {
        bool SendHeaders(List<string> headers);

        int SendBody(byte[] packet, int size);

    }
}