using Giantnodes.Infrastructure.Mail;
using Giantnodes.Infrastructure.Masstransit.Validation;
using Giantnodes.Service.Tenants.Persistance;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Giantnodes.Service.Tenants.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddMailServices(configuration);

            services.AddPersistenceServices(configuration);

            services.AddMassTransitServices(configuration, env);

            return services;
        }

        private static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services
                .AddMassTransit(options =>
                {
                    options
                        .AddConsumers(new[] { Assembly.GetExecutingAssembly() });

                    options
                        .SetKebabCaseEndpointNameFormatter();

                    options.UsingRabbitMq((context, config) =>
                    {
                        config.UseConsumeFilter(typeof(FluentValidationFilter<>), context);
                        config.ConfigureEndpoints(context);
                    });
                });

            return services;
        }
    }
}