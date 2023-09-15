
using FileMoverService;
using HubNet.Log4Net.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileMoverService
{
   public class Program
    {
        public static void Main(string[] args)
        {


            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            IHost host = Host.CreateDefaultBuilder(args)
                    .UseWindowsService()
                    .ConfigureServices((hostContext, services) =>
                    {
                        IConfiguration configuration = hostContext.Configuration;
                        services.AddSingleton(configuration);
                        //hostContext.Configuration.GetSection("");
                        services.AddHostedService<FileMoverWService>();
                    })
                    .Build();
            host.Run();
        }
    }
}

