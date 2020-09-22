using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Receivers.AzureBlob.UnitTests
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
                    {CommonProperties.FileName, config["FileName"]},
                    {CommonProperties.FileExtension, config["FileExtension"]}

                });
            await handler.Initialize();
            var data = await handler.GetFile("20190901082620.csv");  
            var list= await handler.ListFiles();
        }
    }



}
