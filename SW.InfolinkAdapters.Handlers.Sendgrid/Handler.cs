using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Sendgrid
{
    public class Handler: IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.ApiKey); ;
        }
        
        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            var request = JsonConvert.DeserializeObject<EmailRequest>(xchangeFile.Data);
            
            var client = new SendGridClient( Runner.StartupValueOf<string>(CommonProperties.ApiKey));
            
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(request.From),
                Subject = request.Subject,
                TrackingSettings = new TrackingSettings
                {
                    ClickTracking = new ClickTracking { Enable = false }
                },
            };
            msg.AddTo(new EmailAddress(request.To));
            
            if (request.Template != null)
            {
                msg.SetTemplateId(request.Template);
                msg.SetTemplateData(request.TemplateVariables);
            }
            else if (request.Body != null)
            {
                msg.HtmlContent = request.Body;
                msg.PlainTextContent = request.Body;
            }
            else
            {
                throw new Exception("Both Body and Template can not be null.");
            }
            
            
            if (request.AttachmentLocations != null)
            {
                var attachments = new List<Attachment>();
                foreach (var (key, value) in request.AttachmentLocations)
                {
                    var fileBytes = await GetFileBytes(value);
                    attachments.Add(new Attachment
                    {
                        Content = Convert.ToBase64String(fileBytes,0,fileBytes.Length), 
                        Filename = key,
                    });
                }

                msg.Attachments = attachments;
            }

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode < HttpStatusCode.BadRequest) return new XchangeFile(response.StatusCode.ToString());

            throw new Exception(response.StatusCode.ToString());
        }
        
        public async Task<byte[]> GetFileBytes(string url)
        {
            byte[] bytes = Array.Empty<byte>();
            if (url == null || url.Substring(0, 4) != "http") return bytes;
            using (var client = new WebClient())
            {
                try
                {
                    bytes = client.DownloadData(url);
                }catch
                {
                    
                }
                
            }
            return bytes;
        }
    }
}