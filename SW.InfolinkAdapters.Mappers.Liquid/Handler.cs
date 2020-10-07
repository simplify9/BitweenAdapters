using DotLiquid;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Mappers.Liquid
{
    public class Handler : IInfolinkHandler

    {
        public Handler()
        {
            Runner.Expect(CommonProperties.DataTemplate);
            Runner.Expect(CommonProperties.FileNameTemplate, false);
        }

        public Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(xchangeFile.Data, new DictionaryConverter());
            var jsonHash = Hash.FromDictionary(obj);
            
            var templateData = Runner.StartupValueOf(CommonProperties.DataTemplate);
            
            var parsedTemplate = Template.Parse(templateData);
            
            var result = parsedTemplate.Render(jsonHash);

            return Task.FromResult(new XchangeFile(result));

          
        }
    }
}
