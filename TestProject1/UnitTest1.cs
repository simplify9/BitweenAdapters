using SW.InfolinkAdapters.Handlers.Mailgun;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var handler = new Handler();
        Runner.MockRun(handler, new ServerlessOptions(), new Dictionary<string, string>
        {
            { "Url", "https://api.mailgun.net/v3/" },
            {"ApiKey", "YXBpOjE3YjQyOGU3YmY3ZmE0OTdjYWYzNWE3NmU5OWJmZDY5LWQzMmQ4MTdmLTcxMWYxZjk3"}
        });

        var data =
            "{\"To\":\"aram@simplify9.com\",\"From\":\"services@gl-net.com\",\"Subject\":\"Unpaid Shipment COD\",\"AttachmentLocations\":{\"https://nyc3.digitaloceanspaces.com/traxis/unpaid-cod/04/06/2023%2000:00:00-Open\":\"unpaid-cod/04/06/2023 00:00:00-Open\",\"https://nyc3.digitaloceanspaces.com/traxis/unpaid-cod/04/06/2023%2000:00:00-Final.xlsx\":\"unpaid-cod/04/06/2023 00:00:00-Final.xlsx\"},\"Template\":\"-\",\"Body\":\"This is a report for the unpaid shipment CODs in the last week\",\"TemplateVariables\":null}";
        await handler.Handle(new XchangeFile(data));
    }
}