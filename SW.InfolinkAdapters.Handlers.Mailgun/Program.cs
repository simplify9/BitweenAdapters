using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Mailgun
{
    class Program
    {
        private static async Task Main(string[] args) => await Runner.Run(new Handler());

    }
}