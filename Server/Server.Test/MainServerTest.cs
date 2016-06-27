using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class MainServerTest
    {
        [Fact]
        public void Get_Empty_Request()
        {
            var requestProcessor = new MockRequestProcessor();
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties("",
                5555, new ServerTime(),
                new MockPrinter());
            var dirServer = new MainServer(zSocket,
                properties,
                new HttpServiceFactory(new MockHttpService()),
                requestProcessor,
                new List<string> { "Server.Test" },
                new List<Assembly>
                {Assembly.GetAssembly(typeof (MockHttpService))});
            dirServer.Run();
        }

        [Fact]
        public void Web_Server_No_Longer_Taking_Request()
        {
            var requestProcessor = new MockRequestProcessor();
            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var properties = new ServerProperties("",
                5555, new ServerTime(), new MockPrinter());
            var dirServer = new MainServer(zSocket,
                properties,
                new HttpServiceFactory(new MockHttpService()),
                requestProcessor,
                new List<string> { "Server.Test" },
                new List<Assembly>
                {Assembly.GetAssembly(typeof (MockHttpService))});
            dirServer.StopNewConnAndCleanUp();
            dirServer.Run();
            zSocket.VerifyNoAccept();
        }

        [Fact]
        public void Active_Catch_When_Server_Is_Shuting_Down()

        {
            var server = new MainServer(null, null, null, null,
                null, null);
            server.Run();
        }

        [Fact]
        public void Make_Sure_Sever_Is_Still_Alive()
        {
            var requestProcessor = new MockRequestProcessor();
            var mockZSocket = new MockZSocket();
            var properties = new ServerProperties("",
                5555, new ServerTime(), new MockPrinter());
            var dirServer = new MainServer(mockZSocket,
                properties,
                new HttpServiceFactory(new MockHttpService()),
                requestProcessor,
                new List<string> { "Server.Test" },
                new List<Assembly>
                {Assembly.GetAssembly(typeof (MockHttpService))});
            Assert.Equal(true, dirServer.AcceptingNewConn);
        }

        [Fact]
        public void Server_Processing_Request_When_End_Is_Made()
        {
            var requestProcessor = new MockRequestProcessor();
            var serverProperties = new ServerProperties(null,
                5555,
                new ServerTime(), new MockPrinter());

            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET /Sleep HTTP/1.1\r\n")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var server = new MainServer(zSocket, serverProperties,
                new HttpServiceFactory(new MockHttpService()),
                requestProcessor,
                new List<string> { "Server.Test" },
                new List<Assembly>
                {Assembly.GetAssembly(typeof (MockHttpService))});
            var runningServer =
                new Thread(() => server.RunningProcess(
                    new PoolDataForRequest(new HttpResponse(zSocket), 
                    zSocket,
                    Guid.NewGuid())));
            runningServer.Start();
            Thread.Sleep(1000);
            server.StopNewConnAndCleanUp();
            runningServer.Join();
            zSocket.VerifyReceive();
            zSocket.VerifyCloseN(2);
        }

        [Fact]
        public void Make_Web_Server_Get_Request()
        {
            var requestProcessor = new MockRequestProcessor();
            var serverProperties = new ServerProperties(null,
                5555,
                new ServerTime(), new MockPrinter());

            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET / HTTP/1.1\r\n")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var server = new MainServer(zSocket, serverProperties,
                new HttpServiceFactory(new MockHttpService()),
                requestProcessor,
                new List<string> { "Server.Test" },
                new List<Assembly>
                {Assembly.GetAssembly(typeof (MockHttpService))});
            server.RunningProcess(
                new PoolDataForRequest(new HttpResponse(zSocket), 
                zSocket,
                Guid.NewGuid()));
            zSocket.VerifyReceive();

            zSocket.VerifyCloseN(1);
        }

        [Fact]
        public void Mock_Socket_Error()
        {
            var requestProcessor = new MockRequestProcessor();
            var serverProperties = new ServerProperties(null,
                5555,
                new ServerTime(), new MockPrinter());

            var zSocket = new MockZSocket()
                .StubSentToReturn(10)
                .StubReceive("GET /throw HTTP/1.1\r\n")
                .StubConnect(true);
            zSocket = zSocket.StubAcceptObject(zSocket);
            var server = new MainServer(zSocket, serverProperties,
                new HttpServiceFactory(new MockHttpService()),
                requestProcessor,
                new List<string> { "Server.Test" },
                new List<Assembly>
                {Assembly.GetAssembly(typeof (MockHttpService))});
            server.RunningProcess(
                new PoolDataForRequest(new HttpResponse(zSocket),
                zSocket,
                Guid.NewGuid()));
            zSocket.VerifyReceive();

            zSocket.VerifyCloseN(1);
        }
    }
}