using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Receivers.Http
{
    public class Options
    {
        public Options()
        {
            AuthType= Runner.StartupValueOf(CommonProperties.AuthType);
            ApiKey =  Runner.StartupValueOf(CommonProperties.ApiKey);
            LoginUrl = Runner.StartupValueOf(CommonProperties.LoginUrl);
            LoginUsername = Runner.StartupValueOf(CommonProperties.Username);
            LoginPassword = Runner.StartupValueOf(CommonProperties.Password);
            Url = Runner.StartupValueOf(CommonProperties.Url);
            ContentType = Runner.StartupValueOf(CommonProperties.ContentType);
            Headers = Runner.StartupValueOf(CommonProperties.Headers);
            Verb = Runner.StartupValueOf(CommonProperties.Verb);
        }
        
        
        // Auth Types:
        // 1. No Auth (default)
        // 2. ApiKey

        public string AuthType { get; set; }
        public string ApiKey { get; set; }
        public string LoginUrl { get; set; }
        public string Verb { get; set; }
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public string Headers { get; set; }
    }
}