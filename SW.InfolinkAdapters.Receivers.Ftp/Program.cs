using SW.Serverless.Sdk;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Receivers.Ftp
{
    public class Program 
    {

        static async Task Main(string[] args)
        {

            await Runner.Run(new Handler());
        }

    }
}
