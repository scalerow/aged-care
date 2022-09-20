using Giantnodes.Service.Identity.Persistence.Seeders.Abstractions;
using Giantnodes.Service.Identity.Persistence.Seeders.Default;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Giantnodes.Service.Identity.Persistence.Seeders
{
    public static class ApplicationDbContextSeeder
    {
        public static async Task SeedDatabaseAsync(this IServiceScope scope, SeedLevel level, CancellationToken cancellation = default)
        {
            var provider = scope.ServiceProvider;
            var database = provider.GetRequiredService<ApplicationDbContext>();

            await database.Database.MigrateAsync(cancellation);
            if (database.Database.GetDbConnection() is NpgsqlConnection connection)
            {
                await connection.OpenAsync();
                try
                {
                    connection.ReloadTypes();
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            switch (level)
            {
                case SeedLevel.Production:
                    await new OpenIddictSeeder().SeedAsync(database, provider, cancellation);
                    break;

                case SeedLevel.Staging:
                    await new OpenIddictSeeder().SeedAsync(database, provider, cancellation);
                    break;

                case SeedLevel.Development:
                    await new OpenIddictSeeder().SeedAsync(database, provider, cancellation);
                    break;
            }
        }
    }
}
