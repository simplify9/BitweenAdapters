using SW.Serverless.Sdk;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Handlers.S3
{
    public class Program 
    {
        static async Task Main(string[] args)
        {
            //var services = ConfigureServices();

            //var serviceProvider = services.BuildServiceProvider();

            await Runner.Run(new Handler());


        }

        //private static IServiceCollection ConfigureServices()
        //{
        //    IServiceCollection services = new ServiceCollection();

        //    var _config = LoadConfiguration();
        //    services.AddSingleton(_config);

        //    return services;
        //}

        //public static IConfiguration LoadConfiguration()
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("jsconfig1.json", optional: true,
        //                     reloadOnChange: true);
        //    return builder.Build();
        //}
    }
}
