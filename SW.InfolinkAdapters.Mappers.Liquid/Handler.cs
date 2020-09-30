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
            //var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(@"{ ""names"":[{""name"": ""John""},{""name"":""Doe""}]  }", new DictionaryConverter());
            var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(xchangeFile.Data, new DictionaryConverter());
            var jsonHash = Hash.FromDictionary(obj);
            var templatetest = "<h1>{{device}}</h1><h2>{{data.key1}}</h2>{% for client in names %}<h4>{{client.name}}</h4>{% endfor %}";

            var template = Template.Parse(templatetest);
            var render = template.Render(jsonHash);

            return Task.FromResult(new XchangeFile(render));

            //Template template = Template.Parse("hi {{name}}");  // Parses and compiles the template
            //template.Render(Hash.(JsonConvert.DeserializeObject(); // Renders the output => "hi tobi"        }
        }
    }
}
