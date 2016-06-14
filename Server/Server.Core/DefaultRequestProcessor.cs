using System;

namespace Server.Core
{
    public class DefaultRequestProcessor : IRequestProcessor
    {
        public string HandleRequest(string request,
            IZSocket handler, ISend sender,
            IHttpServiceProcessor service,
            ServerProperties serverProperties)
        {
            if (request.Contains("Content-Type: multipart/form-data;"))
                return MultiPart(request,
                    handler, service, serverProperties,
                    sender);
            return NoMultiPart(request,
                handler, service, serverProperties,
                sender);
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
            ISend sender)
        {
            var requestPacket = request;
            var packetSplit = GetPacketSplit(request);
            var httpResponce = serverProperties.DefaultResponse.Clone();

            httpResponce = service.ProcessRequest(requestPacket,
                httpResponce, serverProperties);

            while (!requestPacket.EndsWith($"------{packetSplit}--\r\n"))
            {
                requestPacket = handler.Receive();
                if (httpResponce.HttpStatusCode != "409 Conflict")
                    httpResponce = service
                        .ProcessRequest(requestPacket, httpResponce,
                            serverProperties);
            }
            return sender.SendResponce(handler, httpResponce);
        }

        private string NoMultiPart(string request,
            IZSocket handler,
            IHttpServiceProcessor service,
            ServerProperties serverProperties,
            ISend sender)
        {
            var httpResponce = service.ProcessRequest(request,
                serverProperties.DefaultResponse.Clone(),
                serverProperties);
            return sender.SendResponce(handler, httpResponce);
        }
    }
}