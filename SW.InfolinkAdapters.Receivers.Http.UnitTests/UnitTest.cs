using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Receivers.Http.UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task TestMethod1() 
        {
            var defaultRequest = JsonConvert.SerializeObject(new
            {
                // From = "2024-03-01T12:34:56.789Z",
                // To = "2024-03-26T12:34:56.789Z",
                // Entities = new[] { "ACCT", "PRCL", "BAGM", "MNFT", "CASE" }
            });

            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(), 
                new Dictionary<string, string>
                {
                    { CommonProperties.Url,  "" },
                    { CommonProperties.AuthType, "" },
                    { CommonProperties.LoginUrl, "" },
                    { CommonProperties.Username, "" },
                    { CommonProperties.Password, "" },
                    { CommonProperties.Verb, "POST" },
                    { CommonProperties.DefaultRequest, defaultRequest },
                });

            await handler.Initialize();
            var names = await handler.ListFiles();
            var list = names.ToList();
            foreach (var name in list)
            {
                await handler.GetFile(name);
            }
                
        }
    }
}