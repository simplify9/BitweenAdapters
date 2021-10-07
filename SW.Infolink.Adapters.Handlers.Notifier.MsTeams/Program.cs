using System;
using System.Threading.Tasks;
using SW.Serverless.Sdk;

namespace SW.Infolink.Adapters.Handlers.Notifier.MsTeams
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Runner.Run(new Handler());

        }
    }
}