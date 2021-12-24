using CsvHelper;
using Newtonsoft.Json.Linq;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace SW.InfolinkAdapters.Mappers.JsonToDelimited
{
  public  class Handler : IInfolinkHandler
    
    {
        public Handler()
        {
            
            Runner.Expect(CommonProperties.TargetPath);
            Runner.Expect(CommonProperties.FieldsDelimiter,",");
        }

        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            StringWriter csvString = new StringWriter();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = Runner.StartupValueOf(CommonProperties.FieldsDelimiter)
            };
            using (var csv = new CsvWriter(csvString,  config))
            {

                JToken jToken = JObject.Parse(xchangeFile.Data);
                var doc = jToken.SelectToken(Runner.StartupValueOf(CommonProperties.TargetPath)).ToString();

                if (!doc.StartsWith("["))
                    doc = "[" + doc + "]";

                var jArray = JArray.Parse(doc);
                JObject header = (JObject)(jArray).First();

                foreach (JProperty prop in header.Properties())
                {
                    csv.WriteField(prop.Name);
                }
                csv.NextRecord();


                foreach (JObject item in jArray)
                {
                    var props1 = item.Properties();
                    foreach (JProperty prop in props1)
                    {
                        csv.WriteField(prop.Value.ToString());
                    }
                    csv.NextRecord();
                }
            }

            return new XchangeFile(csvString.ToString(),xchangeFile.Filename);
        }
    }
}
