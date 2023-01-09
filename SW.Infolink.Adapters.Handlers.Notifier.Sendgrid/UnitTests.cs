using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SW.InfolinkAdapters;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.Infolink.Adapters.Handlers.Notifier.Sendgrid;

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
                { "ApiKey", "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" },
                { "From", "autoparts@gig.com.jo" },
                { "To", "abuhussein_ahmad@yahoo.com" },
                { "Subject", "GIG AutoParts || Archiving process failure - فشل في عملية الأرشفة" },
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