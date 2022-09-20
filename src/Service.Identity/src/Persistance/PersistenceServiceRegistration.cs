using Giantnodes.Service.Identity.Persistance.OpenId;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Identity.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddDbContextPool<ApplicationDbContext>(options =>
               {
                   options
                       .UseNpgsql(configuration.GetConnectionString(name: "DatabaseConnection"), o => o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                       .UseSnakeCaseNamingConvention()
                       .UseOpenIddict<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, Guid>();
               });

            return services;
        }
    }
}
