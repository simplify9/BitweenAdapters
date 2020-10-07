using System;
using System.Threading.Tasks;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Mappers.Liquid
{
    class Program
    {
        async static Task Main(string[] args) => await Runner.Run(new Handler());

    }
}
