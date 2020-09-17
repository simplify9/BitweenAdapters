using SW.Serverless.Sdk;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Mappers.JsonToDelimited
{
    public class Program 
    {
        static async Task Main(string[] args)
        {
            await Runner.Run(new Handler());
        }

    }

   
}
