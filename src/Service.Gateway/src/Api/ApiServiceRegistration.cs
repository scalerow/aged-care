using Giantnodes.Service.Gateway.Api.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Giantnodes.Service.Gateway.Api
{
    public static class ApiServiceRegistration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddHttpContextAccessor();

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
            services.Configure<GatewaySettings>(configuration.GetSection("DomainServices"));
            services.AddSingleton<IValidateOptions<GatewaySettings>, GatewaySettingsValidator>();

            // create http clients required to conenct to each of the domain services graphql schema endpoint
            var settings = configuration.Get<GatewaySettings>();
            foreach (var domain in settings.Domains)
            {
                if (string.IsNullOrEmpty(domain.Name))
                    throw new ArgumentNullException(nameof(domain.Name));

                if (string.IsNullOrEmpty(domain.SchemaConnection))
                    throw new ArgumentNullException(nameof(domain.SchemaConnection));

                services.AddHttpClient(domain.Name, c => c.BaseAddress = new Uri(domain.SchemaConnection));
            }

            services
                .AddGraphQLServer()
                .AddQueryType()
                .AddMutationType()
                .AddRemoteSchemasFromRedis(settings.Name, sp => sp.GetRequiredService<ConnectionMultiplexer>());

            return services;
        }
    }
}
