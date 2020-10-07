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
            var result = handler.Handle(new XchangeFile(@"{ ""device"" : ""laptop"", ""data"" : { ""key1"": ""val1"", ""key2"": ""val2"" },  ""names"":[{ ""name"": ""John""},{ ""name"":""Doe""}]  }"));  
            Runner.MockRun(new Handler(), null,new Dictionary<string, string>(CommonProperties.DataTemplate,""))};
        }
    }
}
