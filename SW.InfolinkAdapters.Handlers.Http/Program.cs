using System;
using System.Threading.Tasks;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Http
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Runner.Run(new Handler());

        }
    }
}