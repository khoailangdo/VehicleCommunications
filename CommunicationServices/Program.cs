using CommunicationServices.Models;

namespace CommunicationServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    services.Configure<WorkserSettings>(configuration.GetSection("Settings"));
                    services.AddSingleton<JT808.Protocol.JT808Serializer>();
                })                
                .Build();

            host.Run();
        }
    }
}