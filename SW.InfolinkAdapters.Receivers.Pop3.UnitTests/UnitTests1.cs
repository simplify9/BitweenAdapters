using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Receivers.Pop3.UnitTests
{
    [TestClass]
    public class UnitTests1
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
                    {CommonProperties.Host, config["Host"]},
                    {CommonProperties.Username, config["Username"]},
                    {CommonProperties.Password, config["Password"]},
                    {CommonProperties.TargetPath, config["TargetPath"]},
                    {CommonProperties.LicenseKey, config["LicenseKey"]},
                    {CommonProperties.BatchSize, config["BatchSize"]},
                    {CommonProperties.DeleteMovesFileTo, config["DeleteMovesFileTo"]},
                });
            
            await handler.Initialize();

            var numbers = await handler.ListFiles();

            var msg = await handler.GetFile(numbers.FirstOrDefault());  
            
        }
        
    }
}