using Microsoft.Extensions.Configuration;
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

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }


        [TestMethod]
        async public Task TestMethod1()
        {
            var config = InitConfiguration();
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{
                    {CommonProperties.ConnectionString, config["BlobStorage:ConnectionString"]},
                    {CommonProperties.TargetPath, config["BlobStorage:ContainerName"]},
                     {CommonProperties.FileName, config["BlobStorage:FileName"]},
                      {CommonProperties.FileExtension, config["BlobStorage:FileExtension"]}

                });
            await handler.Handle(new XchangeFile("sss", "testfile.txt"));
        }
    }
}
