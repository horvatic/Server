using System;

namespace Server.Core
{
    public class DefaultRequestProcessor : IRequestProcessor
    {
        public string HandleRequest(string request,
            IZSocket handler,
            IHttpServiceProcessor service,
            ServerProperties serverProperties,
            IHttpResponse httpResponse)
        {
            if (request.Contains("Content-Type: multipart/form-data;"))
                return MultiPart(request,
                    handler, service, serverProperties,
                    httpResponse);
            return NoMultiPart(request,
                handler, service, serverProperties,
                httpResponse);
        }

        private string GetPacketSplit(string request)
        {
            var packetSplit = request.Substring(request.IndexOf("boundary=----"
                , StringComparison.Ordinal) + 13);
            packetSplit = packetSplit.Remove(packetSplit.IndexOf("\r\n"
                , StringComparison.Ordinal));
            return packetSplit;
        }


        private string MultiPart(string request,
            IZSocket handler,
            IHttpServiceProcessor service,
            ServerProperties serverProperties,
            IHttpResponse httpResponse)
        {
            var requestPacket = request;
            var packetSplit = GetPacketSplit(request);
            

            var status = service.ProcessRequest(requestPacket,
                httpResponse, serverProperties);

            while (!requestPacket.EndsWith($"------{packetSplit}--\r\n"))
            {
                requestPacket = handler.Receive();
                if (status != "409 Conflict")
                    status = service
                        .ProcessRequest(requestPacket,
                        httpResponse,
                            serverProperties);
            }
            return status;
        }

        private string NoMultiPart(string request,
            IZSocket handler,
            IHttpServiceProcessor service,
            ServerProperties serverProperties,
            IHttpResponse httpResponse)
        {
            var status = service.ProcessRequest(request,
                httpResponse,
                serverProperties);
            return status;
        }
    }
}