using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestJson()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://postman-echo.com/post" },
                });
            var rs = await handler.Handle(new XchangeFile(JsonConvert.SerializeObject(new
            {
                title = "Some Title.",
                body = "Some body. Long Body.",
                userId = 2
            })));

            var rsVals = JToken.Parse(rs.Data)["json"]["userId"].Value<int>();
            Assert.AreEqual(2, rsVals);
        }

        [TestMethod]
        public async Task TestMultiForm()
        {
            var handler = new Handler();
            var data = "data1,data2,data3,data4";
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://postman-echo.com/post" },
                    { "ContentType", "multipart/form-data" }
                });
            var rs = await handler.Handle(new XchangeFile(data, "file.csv"));
            var b64 = JToken.Parse(rs.Data)["files"]["file.csv"].ToString();
            var b64cleaned = b64.Substring(b64.IndexOf(',') + 1);
            var b64Data = Convert.ToBase64String(Encoding.Default.GetBytes(data));
            Assert.AreEqual(b64cleaned, b64Data);
        }
        
        [TestMethod]
        public async Task TestUrlEncoded()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://postman-echo.com/post" },
                    { "ContentType", "application/x-www-form-urlencoded" }
                });
            var data = JsonConvert.SerializeObject(new
            {
                key1 = "val1",
                key2 = "val2",
            });
            var rs = await handler.Handle(new XchangeFile(data));
            var rsVals = JToken.Parse(rs.Data)["form"].ToString(Formatting.None);
            Assert.AreEqual(rsVals, data);
        }
        
        [TestMethod]
        public async Task TestHeaders()
        {
            string headers = string.Join(',', new string[]
            {
                "Api-Key:Something",
                "SomeHeader:SomeValue"
            });
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://postman-echo.com/post" },
                    { "Headers", headers }
                });
        
            var rs = await handler.Handle(new XchangeFile(""));
        
            var someheader = JToken.Parse(rs.Data)["headers"]["someheader"].Value<string>();
            var apiKey = JToken.Parse(rs.Data)["headers"]["api-key"].Value<string>();
        
        
            Assert.AreEqual(someheader, "SomeValue");
            Assert.AreEqual(apiKey, "Something");
        }
        
        [TestMethod]
        public async Task TestGet()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://postman-echo.com/get?test=1" },
                    { "Verb", "get" }
                });
        
            var rs = await handler.Handle(new XchangeFile(""));
            var test = JToken.Parse(rs.Data)["args"]["test"].Value<string>();
            Assert.AreEqual(test, "1");
        }
        
        [TestMethod]
        public async Task TestGetParameterized()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://postman-echo.com/get?test={{param1}}" },
                    { "Verb", "get" }
                });
        
            var rs = await handler.Handle(new XchangeFile(JsonConvert.SerializeObject(new
            {
                param1 = 1
            })));
            var test = JToken.Parse(rs.Data)["args"]["test"].Value<string>();
            Assert.AreEqual(test, "1");
        }
        
        [TestMethod]
        public async Task TestXML()
        {
            var handler = new Handler();
            Runner.MockRun(handler, new ServerlessOptions(),
                new Dictionary<string, string>
                {
                    { "Url", "https://staging.postaplus.net/ShippingClient/APIServices/ShippingClient.svc" },
                    { "Headers", "SOAPAction:::http://tempuri.org/IShippingClient/Pickup_Creation" },
                    { "ContentType", "text/xml" }
                });
            var data =
                "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:pos=\"http://schemas.datacontract.org/2004/07/ShippingClient\" xmlns:tem=\"http://tempuri.org/\">\n\n  <soapenv:Header />\n\n  <soapenv:Body>\n\n    <tem:Pickup_Creation>\n\n      <tem:PICKUPINFO>\n\n        <pos:ClientInfo>\n\n          <pos:CodeStation>DXB</pos:CodeStation>\n\n          <pos:Password>Testbhagi@123</pos:Password>\n\n          <pos:ShipperAccount>CR23002006</pos:ShipperAccount>\n\n          <pos:UserName>CR22000145</pos:UserName>\n\n        </pos:ClientInfo>\n\n        <pos:CodeService>SRV6</pos:CodeService>\n\n        <pos:ContactPerson>DXBETRACK</pos:ContactPerson>\n\n        <pos:DeliverContactPerson>DXBETRACK</pos:DeliverContactPerson>\n\n        <pos:DeliveryAddress>TEST, , , </pos:DeliveryAddress>\n\n        <pos:DeliveryCity>CITY3856</pos:DeliveryCity>\n\n        <pos:DeliveryPhone>971560065645</pos:DeliveryPhone>\n\n        <pos:GoodsType>-</pos:GoodsType>\n\n        <pos:Notes />\n\n        <pos:PickAddress>TEST, , , </pos:PickAddress>\n\n        <pos:PickCity>CITY3856</pos:PickCity>\n\n        <pos:PickDate>2023-03-09</pos:PickDate>\n\n        <pos:PickPhone>971560065645</pos:PickPhone>\n\n        <pos:PickUpNo>PU230615702</pos:PickUpNo>\n\n        <pos:RequestedPerson>ahmad ibdah</pos:RequestedPerson>\n\n        <pos:TimeEnd>22:00</pos:TimeEnd>\n\n        <pos:TimeOffice>22:00</pos:TimeOffice>\n\n        <pos:TimeStart>00:00</pos:TimeStart>\n\n        <pos:VehicleCode>VHTY1</pos:VehicleCode>\n\n      </tem:PICKUPINFO>\n\n    </tem:Pickup_Creation>\n\n  </soapenv:Body>\n\n</soapenv:Envelope>";
            var rs = await handler.Handle(new XchangeFile(data));
            Assert.AreNotEqual(null, rs);
        }
    }
}