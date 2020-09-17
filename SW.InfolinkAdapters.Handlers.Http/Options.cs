using SW.Serverless.Sdk;

namespace SW.Infolink.Handler.Http
{
    public class Options
    {
        public Options()
        {
            AuthType= Runner.StartupValueOf("AuthType");
            ApiKey =  Runner.StartupValueOf("ApiKey");
            LoginUrl = Runner.StartupValueOf("LoginUrl");
            LoginUsername = Runner.StartupValueOf("LoginUsername");
            LoginPassword = Runner.StartupValueOf("LoginPassword");
            Url = Runner.StartupValueOf("Url");
            ContentType = Runner.StartupValueOf("ContentType");
        }
        
        
        // Auth Types:
        // 1. No Auth (default)
        // 2. ApiKey
        // 3. Login

        public string AuthType { get; set; }
        public string ApiKey { get; set; }
        public string LoginUrl { get; set; }
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
    }
    
}