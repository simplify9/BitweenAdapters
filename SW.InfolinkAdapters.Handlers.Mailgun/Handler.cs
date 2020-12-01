using System.Collections.Generic;
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
            MailGunSendRequest rq = JsonConvert.DeserializeObject<MailGunSendRequest>(xchangeFile.Data);

            if (rq.Template != null)
            {
                
            }
            else if (rq.Body != null)
            {
                
            }
            else
            {
                
            }
            
        }
    }
}