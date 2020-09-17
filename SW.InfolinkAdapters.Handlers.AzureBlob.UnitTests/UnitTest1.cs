using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Handlers.AzureBlob.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        async public Task TestMethod1()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    {"BlobStorageAdaptor.ContainerName", "customers-out-dev"},
                    {"BlobStorageAdaptor.FileExtension", "csv" }           
                
                });
            await handler.Handle(new XchangeFile("sss", "testfile.txt"));
        }
    }
}
