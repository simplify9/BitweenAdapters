using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestJson()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    {"Url", "https://jsonplaceholder.typicode.com/posts" },
                });
            await handler.Handle(new XchangeFile(JsonConvert.SerializeObject(new
            {
                title = "Some Title.",
                body = "Some body. Long Body.",
                userId = 2
            }), "testfile.txt"));

        }
        [TestMethod]
        public async Task TestMultiForm()
        {
            var handler = new Handler();
            var data = "data1,data2,data3,data4";
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    {"Url", "https://postman-echo.com/post" },
                    {"ContentType", "multipart/form-data"}
                });
            var rs =await handler.Handle(new XchangeFile(data, "file.csv"));
            var b64 = JToken.Parse(rs.Data)["files"]["file.csv"].ToString();
            var b64cleaned = b64.Substring(b64.IndexOf(',') + 1);
            var b64Data = Convert.ToBase64String(Encoding.Default.GetBytes(data));
            Assert.AreEqual(b64cleaned, b64Data);
            

        }
    }
}