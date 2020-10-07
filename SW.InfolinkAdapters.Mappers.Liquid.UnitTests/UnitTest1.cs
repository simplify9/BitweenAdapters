using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Mappers.Liquid.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var handler = new Handler();
            string data =
                @"{ ""device"" : ""laptop"", ""data"" : { ""key1"": ""val1"", ""key2"": ""val2"" },  ""names"":[{ ""name"": ""John""},{ ""name"":""Doe""}]  }";
            Runner.MockRun(new Handler(), null,
                new Dictionary<string, string>
                {
                    [CommonProperties.DataTemplate] =@"<h1>{{device}}</h1><h2>{{data.key1}}</h2>{% for client in names %}<h4>{{client.name}}</h4>{% endfor %}"
                });
            var rs = handler.Handle(new XchangeFile(data));
            Assert.AreEqual(rs.Result.Data, "<h1>laptop</h1><h2>val1</h2><h4>John</h4><h4>Doe</h4>");
        }
    }
}
