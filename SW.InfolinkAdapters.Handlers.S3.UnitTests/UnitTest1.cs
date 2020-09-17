using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Handlers.S3.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        async public Task TestMethod1()
        {
            var handler = new Handler();
            await handler.Handle(new XchangeFile("sss", "testfile.txt"));
        }
    }
}
