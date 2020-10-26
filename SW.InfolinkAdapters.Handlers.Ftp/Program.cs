using System.Threading.Tasks;
using SW.Serverless.Sdk;

namespace SW.Infolink.SftpFileHandler
{
    
    public class Program 
    {

        static async Task Main(string[] args)
        {
            await Runner.Run(new Handler());

        }

    }


}

