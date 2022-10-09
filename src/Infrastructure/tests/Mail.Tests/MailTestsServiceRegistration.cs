using Bogus;
using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Infrastructure.Mail.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Giantnodes.Infrastructure.Mail
{
    public static class MailTestsServiceRegistration
    {
        public static IServiceCollection AddMailTestsServices(this IServiceCollection services)
        {
            var settings = new EmailSettings
            {
                Host = new Faker().Internet.DomainName(),
                Port = new Faker().Internet.Port(),
                Username = new Faker().Internet.UserName(),
                Password = new Faker().Internet.Password()
            };

            services.AddSingleton<IOptions<EmailSettings>>(Options.Create(settings));
            services.AddSingleton<IValidateOptions<EmailSettings>, EmailSettingsValidator>();
            services.AddSingleton<ITemplateRenderService, TemplateRenderService>();
            services.AddSingleton<IEmailService>(new Mock<IEmailService>().Object);

            services.AddTransient<ISmtpClient, SmtpClient>();

            return services;
        }
    }
}
