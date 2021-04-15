using System;
using System.Threading.Tasks;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Sendgrid
{
    class Program
    {
        private static async Task Main(string[] args) => await Runner.Run(new Handler());
    }
}