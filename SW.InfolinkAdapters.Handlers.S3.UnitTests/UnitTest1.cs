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
                    {CommonProperties.LicenseKeySecret, config["CloudFiles:SecretAccessKey"]},
                    {CommonProperties.LicenseKey, config["CloudFiles:AccessKeyId"]},
                    {CommonProperties.TargetPath, config["CloudFiles:BucketName"]},
                    {CommonProperties.Url, config["CloudFiles:ServiceUrl"]},
                    {CommonProperties.FileName, config["FileName"]},
                    {CommonProperties.FileExtension, config["FileExtension"]},
                    {CommonProperties.FolderName, config["FolderName"]},
                    {CommonProperties.ContentType, config["ContentType"]}

                });
            await handler.Handle(new XchangeFile("sss", "testfile.txt"));
        }
    }
}
