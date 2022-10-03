using Giantnodes.Infrastructure.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Tenants.Application.Tests
{
    public static class ApplicationTestsServiceRegistration
    {
        public static IServiceCollection AddApplicationTestServices(this IServiceCollection services)
        {
            services.AddMailTestsServices();
            return services;
        }
    }
}
