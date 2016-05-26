using System;
using System.IO;
using Server.Core;
using Xunit;

namespace Server.Test
{
    public class ClosingServerHandlerTest
    {
        [Fact]
        public void New_Class_Is_Not_Null()
        {
            var closeProcessor = new ClosingServerHandler(new MockMainServer());
            Assert.NotNull(closeProcessor);
        }

        [Fact]
        public void Close_Program()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var testServer = new MockMainServer();
            var closeProcessor = new ClosingServerHandler(testServer);
            closeProcessor.ShutdownProcess(null, null);
            testServer.VerifyStopNewConn();
            testServer.VerifyCleanUp();
            output.Close();


        }
    }
}
