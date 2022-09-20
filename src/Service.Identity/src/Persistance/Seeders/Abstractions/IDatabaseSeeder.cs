using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Identity.Persistence.Seeders.Abstractions
{
    public interface IDatabaseSeeder
    {
        public Task SeedAsync(ApplicationDbContext database, IServiceProvider provider, CancellationToken cancellation = default);
    }
}
