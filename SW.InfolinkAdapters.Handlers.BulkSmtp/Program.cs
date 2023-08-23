using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.BulkSmtp;

class Program
{
    static async Task Main(string[] args)
    {
        await Runner.Run(new Handler());
    }
}