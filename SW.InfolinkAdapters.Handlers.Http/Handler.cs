using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http
{
    public class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.AuthType,null);
            Runner.Expect(CommonProperties.ApiKey, null);
            Runner.Expect(CommonProperties.LoginUrl,null);
            Runner.Expect(CommonProperties.Username,null);
            Runner.Expect(CommonProperties.Password,null);
            Runner.Expect(CommonProperties.Url);
            Runner.Expect(CommonProperties.ContentType, "application/json");
        }
        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            var options = new Options();
            
            var client = new HttpClient();
            
            if (options.AuthType == "ApiKey")
            {
                client.DefaultRequestHeaders.Add("ApiKey", options.ApiKey);
            }
            else if (options.AuthType == "Login")
            {
                var loginJson = JsonConvert.SerializeObject(new UserLoginModel()
                {
                    Email = options.LoginUsername,
                    Password = options.LoginPassword
                });
                
                var loginResponse = await client.PostAsync(new Uri(options.LoginUrl), new StringContent(loginJson, Encoding.UTF8, "application/json"));

                loginResponse.EnsureSuccessStatusCode();

                if (loginResponse.StatusCode == HttpStatusCode.OK)
                {
                    var jwt = await loginResponse.Content.ReadAsStringAsync();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                }
                else
                {
                    throw new Exception(loginResponse.StatusCode.ToString());
                }
            }

            HttpContent content;
            switch (options.ContentType)
            {
                case "multipart/form-data":
                    MultipartFormDataContent multipartTmp = new MultipartFormDataContent();
                    byte[] fileContent = Encoding.UTF8.GetBytes(xchangeFile.Data);
                    multipartTmp.Add(new ByteArrayContent(fileContent), "file", "file.csv");
                    content = multipartTmp;
                    break;
                case "application/json":
                    content = new StringContent(xchangeFile.Data, Encoding.UTF8, options.ContentType);
                    break;
                default:
                    content = new StringContent(xchangeFile.Data, Encoding.UTF8, options.ContentType);
                    break;
            }
            
            var response = await client.PostAsync(new Uri(options.Url), content);
            
            //response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var resp = await response.Content.ReadAsStringAsync();
                return new XchangeFile(resp);
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }   
        }
    }
}