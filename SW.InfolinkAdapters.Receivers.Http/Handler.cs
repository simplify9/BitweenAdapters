using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Receivers.Http
{
    public class Handler:IInfolinkReceiver
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.AuthType, null);
            Runner.Expect(CommonProperties.ApiKey, null);
            Runner.Expect(CommonProperties.LoginUrl, null);
            Runner.Expect(CommonProperties.Username, null);
            Runner.Expect(CommonProperties.Password, null);
            Runner.Expect(CommonProperties.Url);
            Runner.Expect(CommonProperties.Headers, null);
            Runner.Expect(CommonProperties.ContentType, "application/json");
            Runner.Expect(CommonProperties.Verb, "get");
        }
        private HttpMethod HttpMethodFromString(string method)
        {
            switch (method.ToLower())
            {
                case "get":
                    return HttpMethod.Get;
                case "delete":
                    return HttpMethod.Delete;
                case "put":
                    return HttpMethod.Put;
                case "post":
                default:
                    return HttpMethod.Post;
            }
        }
        
        
        
        public async Task Initialize()
        {
            await Task.FromResult(new { });
        }

        public async Task Finalize()
        {
            await Task.FromResult(new { });
        }

        public async Task<IEnumerable<string>> ListFiles()
        {
            return await Task.FromResult(new[] {"1"});
        }

        public async Task<XchangeFile> GetFile(string fileId)
        {
            var options = new Options();

            var client = new HttpClient();

            if (options.AuthType == "ApiKey")
            {
                client.DefaultRequestHeaders.Add("ApiKey", options.ApiKey);
            }
            else if (options.AuthType == "Bearer")
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.LoginPassword);
                
            }
            
            HttpContent content;
           
            var uri = new Uri(options.Url);
           

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethodFromString(options.Verb),
            };

            var headers = options.Headers?.Split(',').Select(h =>
            {
                var split = h.Split(':');
                return new KeyValuePair<string, string>(split[0], split[1]);
            });

            if (headers != null)
                foreach (var keyValuePair in headers)
                    request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

            var response = await client.SendAsync(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 500)
            {
                var resp = await response.Content.ReadAsStringAsync();

                return (int)response.StatusCode < 400 ?
                    new XchangeFile(resp) :
                    new XchangeFile(resp, badData: true);
            }

            throw new Exception(response.StatusCode.ToString());
        }

        public async Task DeleteFile(string fileId)
        {
            await Task.FromResult(new { });
        }
    }
}