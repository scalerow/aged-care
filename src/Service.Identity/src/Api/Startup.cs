using Giantnodes.Service.Identity.Application;

namespace Giantnodes.Service.Identity.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_configuration, _environment);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (!_environment.IsDevelopment())
                app.UseHttpsRedirection();

            app
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoint =>
                {
                    endpoint.MapControllers();
                });
        }
    }
}
