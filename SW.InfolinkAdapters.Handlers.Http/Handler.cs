using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http
{
    public class Handler : IInfolinkHandler
    {
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
            Runner.Expect(CommonProperties.Verb, "post");
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
                        new AuthenticationHeaderValue("Bearer", jwt);
                }
                else
                {
                    throw new Exception(loginResponse.StatusCode.ToString());
                }
            }

            HttpContent content;
            switch (options.ContentType.ToLower())
            {
                case "application/x-www-form-urlencoded":
                    content = new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(xchangeFile.Data));
                    break;
                case "multipart/form-data":
                    MultipartFormDataContent multipartTmp = new MultipartFormDataContent();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(xchangeFile.Data);
                    byte[] fileContent = Encoding.UTF8.GetBytes(data["fileContent"]);
                    multipartTmp.Add(new ByteArrayContent(fileContent), "file", xchangeFile.Filename ?? "file");
                    content = multipartTmp;
                    break;
                case "application/json":
                    content = new StringContent(xchangeFile.Data, Encoding.UTF8, "application/json");
                    break;
                default:
                    content = new StringContent(xchangeFile.Data, Encoding.UTF8, options.ContentType);
                    break;
            }

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(options.Url),
                Method = HttpMethodFromString(options.Verb),
                Content = content,
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




            //if (response.IsSuccessStatusCode)
            //{
            //    var resp = await response.Content.ReadAsStringAsync();
            //    return new XchangeFile(resp);
            //}
            //else
            //{
            //    throw new Exception(response.StatusCode.ToString());
            //}



        }
    }
}