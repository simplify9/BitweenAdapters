using System;
using System.Collections.Generic;
using System.Linq;
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
                    {"Url", "https://postman-echo.com/post" },
                });
            var rs = await handler.Handle(new XchangeFile(JsonConvert.SerializeObject(new
            {
                title = "Some Title.",
                body = "Some body. Long Body.",
                userId = 2
            })));

            var rsVals = JToken.Parse(rs.Data)["json"]["userId"].Value<int>();
            Assert.AreEqual(2, rsVals);

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

        [TestMethod]
        public async Task TestUrlEncoded()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    {"Url", "https://postman-echo.com/post" },
                    {"ContentType", "application/x-www-form-urlencoded"}
                });
            var data = JsonConvert.SerializeObject(new
            {
                key1 = "val1",
                key2 = "val2",
            });
            var rs =await handler.Handle(new XchangeFile(data));
            var rsVals = JToken.Parse(rs.Data)["form"].ToString(Formatting.None);
            Assert.AreEqual(rsVals, data);
            
        }

        [TestMethod]
        public async Task TestHeaders()
        {
            string headers = string.Join(',', new string[]
            {
                "Api-Key:Something",
                "SomeHeader:SomeValue"
            });
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    {"Url", "https://postman-echo.com/post" },
                    {"Headers", headers}
                });
            
            var rs =await handler.Handle(new XchangeFile(""));
            
            var someheader = JToken.Parse(rs.Data)["headers"]["someheader"].Value<string>();
            var apiKey = JToken.Parse(rs.Data)["headers"]["api-key"].Value<string>();

            
            Assert.AreEqual(someheader, "SomeValue");
            Assert.AreEqual(apiKey, "Something");
        }

        [TestMethod]
        public async Task TestGet()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    {"Url", "https://postman-echo.com/get" },
                    {"Verb", "get"}
                });
            
            var rs =await handler.Handle(new XchangeFile(""));
            
            
        }
    }
}