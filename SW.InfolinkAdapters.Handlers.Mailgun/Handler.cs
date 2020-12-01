using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Mailgun
{
    public class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect("ApiKey");
            Runner.Expect("MailgunEndpoint");
            
        }
        
        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            IList<IEnumerable<byte>> attachmentsData = new List<IEnumerable<byte>>();
            MailGunSendRequest mailgunRequest = JsonConvert.DeserializeObject<MailGunSendRequest>(xchangeFile.Data);
            
            
            HttpClient client = new HttpClient();
            string key = Runner.StartupValueOf<string>("ApiKey");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", key );
            
            MultipartFormDataContent formContent = new MultipartFormDataContent();
            
            formContent.Add(new StringContent(mailgunRequest.To), "to");
            formContent.Add(new StringContent(mailgunRequest.From), "from");
            formContent.Add(new StringContent(mailgunRequest.Subject), "subject");
            
            

            if (mailgunRequest.Template != null)
            {
                formContent.Add(new StringContent(mailgunRequest.Template), "template");
                formContent.Add(new StringContent(JsonConvert.SerializeObject(mailgunRequest.TemplateVariables)), "h:X-Mailgun-Variables");
                
                
            }
            else if (mailgunRequest.Body != null)
            {
                
                formContent.Add(new StringContent(mailgunRequest.Template), "body");
            }
            else
            {
                throw new Exception();
            }

            if (mailgunRequest.AttachmentLocations != null)
            {
                foreach (string location in mailgunRequest.AttachmentLocations)
                {
                    Stream stream = await client.GetStreamAsync(location);
                    formContent.Add(new StreamContent(stream), "attachment");
                }
            }

            string mailgunEndpoint = Runner.StartupValueOf<string>("MailgunEndpoint");
            
            HttpResponseMessage message = await client.PostAsync(mailgunEndpoint, formContent);
            
            return new XchangeFile(await message.Content.ReadAsStringAsync());
            
        }
    }
}