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
            {"ApiKey", ""},
            {"Domain", ""}
        });

        //"unpaid-cod/2023-04-06-Open": "https://nyc3.digitaloceanspaces.com/traxis/unpaid-cod/2023-04-06-Open
        var data =
            "{\"To\":\"aram@simplify9.com\",\"From\":\"services@gl-net.com\",\"Subject\":\"Unpaid Shipment COD\",\"AttachmentLocations\":{\"temp1/7aa5037e4ab3484e9dcbe648d8770498\":\"https://nyc3.digitaloceanspaces.com/traxis/temp1/7aa5037e4ab3484e9dcbe648d8770498\",\"temp1/2df4952548df493b8ac04792460840f6\":\"https://nyc3.digitaloceanspaces.com/traxis/temp1/2df4952548df493b8ac04792460840f6\"},\"Template\":null,\"Body\":\"This is a report for the unpaid shipment CODs in the last week\",\"TemplateVariables\":null}";
        await handler.Handle(new XchangeFile(data));
    }
}