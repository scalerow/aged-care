using Giantnodes.Infrastructure.GraphQL;
using Giantnodes.Service.Identity.Persistence;
using HotChocolate.Types.Descriptors;
using StackExchange.Redis;

namespace Giantnodes.Service.Identity.Api
{
    public static class ApiServiceRegistration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddControllers();

            services.AddRedisServices(configuration, environment);
            services.AddGraphQLServices(configuration, environment);

            return services;
        }

        private static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddSingleton(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));

            return services;
        }

        private static IServiceCollection AddGraphQLServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services
                .AddGraphQLServer()
                .InitializeOnStartup()
                .PublishSchemaDefinition(definition =>
                    definition
                        .SetName("identity")
                        .PublishToRedis("gateway", sp => sp.GetRequiredService<ConnectionMultiplexer>()))
                .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
                .AddConvention<INamingConventions, SnakeCaseNamingConvention>()
                .AddQueryType()
                .AddMutationType()
                .AddApiTypes();


            return services;
        }
    }
}
