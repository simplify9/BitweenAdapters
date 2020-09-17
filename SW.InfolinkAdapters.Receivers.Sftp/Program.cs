using SW.Serverless.Sdk;
using System.Threading.Tasks;

namespace SW.Infolink.SftpFileReceiver
{
    public class Program 
    {

        static async Task Main(string[] args)
        {
            await Runner.Run(new Handler());
        }

    }
}
