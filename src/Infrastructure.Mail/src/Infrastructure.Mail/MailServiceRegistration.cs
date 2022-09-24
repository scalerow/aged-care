using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Infrastructure.Mail.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Giantnodes.Infrastructure.Mail
{
    public static class MailServiceRegistration
    {
        public static IServiceCollection AddMailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddSingleton<IValidateOptions<EmailSettings>, EmailSettingsValidator>();

            services.AddSingleton<ITemplateRenderService, TemplateRenderService>();

            services.AddTransient<ISmtpClient, SmtpClient>();
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
