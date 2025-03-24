using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Ftp.UnitTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var handler = new Handler();
        Runner.MockRun(handler, new ServerlessOptions(),
            new Dictionary<string, string>{ 
                {"Host", "" },
                {"PrivateKey",""},
                {"Password",""},
                {"Username",""},
                {"Protocol",""},
            });
        var rs = await handler.Handle(new XchangeFile(""));
        var response = rs;
    }
}