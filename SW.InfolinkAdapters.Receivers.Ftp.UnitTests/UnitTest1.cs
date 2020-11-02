using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Receivers.Ftp.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
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
                    {CommonProperties.Username , config["Username"]},
                    {CommonProperties.Password , config["Password"]},
                    {CommonProperties.LicenseKey , config["License"]},
                    {CommonProperties.Protocol , "ftp" },
                    {CommonProperties.Host , config["Host"]},
                    {CommonProperties.TargetPath, "/httpdocs/RemovableTraxisTraces"}

                });
            await handler.Initialize();

            var list = await handler.ListFiles();

            var data = await handler.GetFile(list.First());
        }
    }



}
