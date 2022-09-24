using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Identity.Application.Tests
{
    public static class ApplicationTestsServiceRegistration
    {
        public static IServiceCollection AddApplicationTestServices(this IServiceCollection services)
        {
            services.AddIdentityServices(default!, default!);
            return services;
        }
    }
}
