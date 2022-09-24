using Giantnodes.Infrastructure.Masstransit.Validation;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Persistance.OpenId;
using Giantnodes.Service.Identity.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using System.Reflection;

namespace Giantnodes.Service.Identity.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddQuartzServices(configuration, env);
            services.AddIdentityServices(configuration, env);
            services.AddOpenIddictServices(configuration, env);

            return services;
        }

        public static IServiceCollection AddQuartzServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            services
                .Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 0;

                    options.User.RequireUniqueEmail = true;

                    options.SignIn.RequireConfirmedEmail = true;
                });

            return services;
        }

        public static IServiceCollection AddOpenIddictServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services
                .AddAuthentication(options => options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

            services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                });

            services
               .Configure<IdentityOptions>(options =>
               {
                   options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                   options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                   options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
               });

            services
                .AddOpenIddict()
                .AddCore(options =>
                {
                    options
                        .UseQuartz();

                    options
                        .UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>()
                        .ReplaceDefaultEntities<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, Guid>();
                })
                .AddServer(options =>
                {
                    options
                        .UseAspNetCore();

                    options
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow();

                    options
                        .SetTokenEndpointUris("/connect/token")
                        .SetLogoutEndpointUris("/connect/logout")
                        .SetIntrospectionEndpointUris("/connect/introspect")
                        .SetUserinfoEndpointUris("/connect/userinfo");

                    options
                        .RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.Roles);

                    options
                        .SetRefreshTokenReuseLeeway(TimeSpan.Zero);
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            return services;
        }

        private static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMassTransit(options =>
                {
                    options
                        .AddConsumers(new[] { Assembly.GetExecutingAssembly(), Assembly.Load("Giantnodes.Dashboard.Api") });

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