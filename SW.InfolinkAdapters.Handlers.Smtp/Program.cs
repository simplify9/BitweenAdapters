using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Smtp;

class Program
{
    static async Task Main(string[] args)
    {
        await Runner.Run(new Handler());
    }
}