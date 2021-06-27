using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SW.InfolinkAdapters;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.Infolink.Adapters.Handlers.Notifier.MsTeams
{
    public class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.Url);
            Runner.Expect(CommonProperties.RedirectionUrl,null);
        }

        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            var redirectionUrl = Runner.StartupValueOf(CommonProperties.RedirectionUrl);

            var url = Runner.StartupValueOf(CommonProperties.Url);

            var model = JsonConvert.DeserializeObject<NotificationModel>(xchangeFile.Data);

            var message = new MsTeamsModel
            {
                Context = "http://schema.org/extensions",
                type = "AdaptiveCard",
                version = "1.0",
                padding = "None",
                summary = "Infolink notification",
                sections = new List<Section>
                {
                  new Section
                  {
                     
                      activityTitle= $"Xchange {model.Id} was processed",
                      activitySubtitle= "Check its details",
                      activityImage= "https://teamsnodesample.azurewebsites.net/static/img/image5.png",
                      markdown= true,
                      facts=  new List<Fact>
                      {
                          new Fact
                          {
                              name = "ID",
                              value = $"{model.Id}"
                          },
                          new Fact
                          {
                              name = "Status",
                              value = model.Success ? "Succeeded" : "Failed"
                          },
                          new Fact
                          {
                              name = "Finished On",
                              value = $"{model.FinishedOn}"
                          },
                          new Fact
                          {
                              name = "Bad Output",
                              value = $"{model.OutputBad}"
                          },
                          new Fact
                          {
                              name = "Bad Response",
                              value = $"{model.ResponseBad}"
                          },
                          new Fact
                          {
                              name = "Exception",
                              value = $"{model.Exception}"
                          }
                      },
                    
                
                  }  
                },
                potentialAction = new List<PotentialAction>
                {
                    new PotentialAction
                    {
                        name = "View in infolink portal",
                        type =  "OpenUri",
                        targets = new List<Target>
                        {
                            new Target
                            {
                                os = "default",
                                uri = redirectionUrl
                            }
                        }
                    }
                }
            };

            

            var serMsg = JsonConvert.SerializeObject(message,
                new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});


            var client = new HttpClient();


            var response = await client.PostAsync(url, new StringContent(serMsg, Encoding.UTF8, "application/json"));

            Console.WriteLine(response.StatusCode);

            return new XchangeFile(response.ReasonPhrase, null, response.IsSuccessStatusCode);
        }

        public class payloadAction
        {
            [JsonProperty("@type")] public string type { get; set; }
            public string name { get; set; }
            public object[] targets { get; set; }
        }
    }
}