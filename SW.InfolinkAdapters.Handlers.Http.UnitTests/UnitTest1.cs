using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>{ 
                    { "AuthType", "Login" },
                    {"LoginUrl", "https://dev-api.kwick-box.com/api/Integration/Login"},
                    {"LoginUsername", "samer.zughul@hotmail.com" },
                    {"LoginPassword", "P@ssw0rd" },
                    {"Url", "https://dev-api.kwick-box.com/api/Integration/SubmitShippingRequests" },
                });
            await handler.Handle(new XchangeFile("[{\"RecipientCity\":\"Amman\",\"RecipientArea\":\"Hona\",\"AddressDescription\":\"ESCALADA 1570 OF. 103, COMODORO RIVADAVIA CHUBUT COMODORO RIVADAVIA, COMODORO RIVADAVIA\",\"RecipientName\":\"MARCELO FRYDLEWICZ\",\"RecipientEmail\":\"patagonia@iram.org.ar\",\"RecipientPhoneNumber\":\"5.40297E+12\",\"CODAmount\":null,\"ItemDetails\":null,\"ReferenceID\":\"1310562532\",\"ItemWeight\":\"0.5\",\"ItemDimension\":\"35*27.5*2\",\"PieceId\":\"JD014600007537950087\"}]", "testfile.txt"));
        }
    }
}