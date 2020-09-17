using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Mappers.JsonToDelimited.UnitTests
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
                    { "BlobStorageAdaptor.ConnectionString", "DefaultEndpointsProtocol=https;AccountName=bcrm1;AccountKey=n+taOdDuZf9nJxp13qO/C/YF1tMH23ulnDrKIHzKZa+/W0kQztWFhNQs1K0Zvq+21YSVu/IQAtxi+Wzuju0TAg==;EndpointSuffix=core.windows.net" },
                    {"BlobStorageAdaptor.ContainerName", "customers-out-dev"},
                    {"BlobStorageAdaptor.FileExtension", "csv" }           
                
                });
            await handler.Handle(new XchangeFile("sss", "testfile.txt"));
        }
    }
}
