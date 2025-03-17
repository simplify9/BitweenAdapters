using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Ftp.UnitTest;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var handler = new Handler();
        Runner.MockRun(handler, new ServerlessOptions(),
            new Dictionary<string, string>{ 
                {"Host", "31231" },
                {"PrivateKey","wgwegwegwegweg"},
                {"Password","wegwdxs"},
                {"Username","werweweff"},
                {"Protocol","sftpssh"},
            });
        var rs = await handler.Handle(new XchangeFile(""));
        Assert.IsFalse(rs.BadData);
    }
}