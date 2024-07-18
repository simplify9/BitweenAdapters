using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SW.InfolinkAdapters;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.Infolink.Adapters.Handlers.Notifier.Sendgrid.UnitTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public async Task TestJson()
    {
        var handler = new Handler();
        Runner.MockRun(handler, new ServerlessOptions(),
            new Dictionary<string, string>
            {
                { "ApiKey", "" },
                { "From", "" },
                { "To", "" },
                { "Subject", "" },
            });
        var rs = await handler.Handle(new XchangeFile(JsonConvert.SerializeObject(new NotificationModel
        {
            Id = "34389",
            Success = true,
            Exception = "sadhjfgdjkshak",
            FinishedOn = DateTime.Now,
            OutputBad = false,
            ResponseBad = false
        })));


        var response = rs;
    }
}