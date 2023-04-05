// See https://aka.ms/new-console-template for more information

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SW.InfolinkAdapters.Handlers.Smtp;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

[TestClass]
public class SmtpTests
{
    [TestMethod]
    public async Task TestBasic()
    {
        var handler = new Handler();
        Runner.MockRun(handler, new ServerlessOptions(),
            new Dictionary<string, string>
            {
                { "Host", "" },
                { "From", "" },
                { "Password", "" },
                { "Port", "" },
            });

        var rs = await handler.Handle(new XchangeFile(JsonConvert.SerializeObject(new InputModel
        {
            Subject = "Smtp test",
            Body = "Hi",
            To = ""
        })));

        Assert.AreEqual("", rs.Data);
    }
}