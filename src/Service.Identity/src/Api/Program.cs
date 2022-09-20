using Giantnodes.Service.Identity.Persistence.Seeders;
using Giantnodes.Service.Identity.Persistence.Seeders.Abstractions;

namespace Giantnodes.Service.Identity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var scope = host.Services.CreateScope();
            var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var level = environment?.EnvironmentName switch
            {
                "Development" => SeedLevel.Development,
                "Staging" => SeedLevel.Staging,
                "Production" => SeedLevel.Production,
                _ => SeedLevel.Production
            };

            await scope.SeedDatabaseAsync(level);
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }
}