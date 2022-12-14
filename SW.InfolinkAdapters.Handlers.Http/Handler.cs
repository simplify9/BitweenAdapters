using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http
{
    public class Handler : IInfolinkHandler
    {
        private HttpMethod HttpMethodFromString(string method)
        {
            return method.ToLower() switch
            {
                "get" => HttpMethod.Get,
                "delete" => HttpMethod.Delete,
                "put" => HttpMethod.Put,
                "post" => HttpMethod.Post,
                _ => HttpMethod.Post
            };
        }

        public Handler()
        {
            Runner.Expect(CommonProperties.Timeout, null);
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


            if (options.Timeout != null)
            {
                client.Timeout = TimeSpan.FromSeconds(options.Timeout.Value);
            }

            switch (options.AuthType)
            {
                case "ApiKey":
                    client.DefaultRequestHeaders.Add("ApiKey", options.ApiKey);
                    break;
                case "Bearer":
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", options.LoginPassword);
                    break;
                case "Login":
                {
                    var loginJson = JsonConvert.SerializeObject(new UserLoginModel()
                    {
                        Email = options.LoginUsername,
                        Password = options.LoginPassword
                    });

                    var loginResponse = await client.PostAsync(new Uri(options.LoginUrl),
                        new StringContent(loginJson, Encoding.UTF8, "application/json"));

                    loginResponse.EnsureSuccessStatusCode();

                    if (loginResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var rs = await loginResponse.Content.ReadAsStringAsync();
                        LoginResponse rsDeserialized = JsonConvert.DeserializeObject<LoginResponse>(rs);
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", rsDeserialized.Jwt);
                    }
                    else
                    {
                        throw new Exception(loginResponse.StatusCode.ToString());
                    }

                    break;
                }
            }

            HttpContent content;
            switch (options.ContentType.ToLower())
            {
                case "application/x-www-form-urlencoded":
                    content = new FormUrlEncodedContent(
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(xchangeFile.Data));
                    break;
                case "multipart/form-data":
                    MultipartFormDataContent multipartTmp = new MultipartFormDataContent();
                    byte[] fileContent = Encoding.UTF8.GetBytes(xchangeFile.Data);
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

            Uri uri = null;
            if (!string.IsNullOrEmpty(xchangeFile.Data) && options.Url.Contains("{{"))
            {
                string templateData = Runner.StartupValueOf(CommonProperties.Url);
                Template parsedTemplate = Template.Parse(templateData);
                IDictionary<string, object> obj =
                    JsonConvert.DeserializeObject<IDictionary<string, object>>(xchangeFile.Data,
                        new DictionaryConverter());
                Hash jsonHash = Hash.FromDictionary(obj);
                uri = new Uri(parsedTemplate.Render(jsonHash));
            }
            else uri = new Uri(options.Url);

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
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

            request.Headers.Add(RequestContext.CorrelationIdHeaderName, options.CorrelationId);
            var response = await client.SendAsync(request);

            if ((int) response.StatusCode < 200 || (int) response.StatusCode >= 500)
                throw new Exception(response.StatusCode.ToString());
            var resp = await response.Content.ReadAsStringAsync();

            return (int) response.StatusCode < 400 ? new XchangeFile(resp) : new XchangeFile(resp, badData: true);
        }
    }
}