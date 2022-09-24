using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Infrastructure.Mail.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using System.Net;

namespace Giantnodes.Infrastructure.Mail.Tests.Mocks
{
    public class EmailServiceMock : IEmailService
    {
        private readonly EmailService _service;

        public EmailSettings Settings { get; init; }
        public Mock<ISmtpClient> Client { get; init; }
        public Mock<ITemplateRenderService> Renderer { get; init; }

        public EmailServiceMock(IOptions<EmailSettings> settings)
        {
            Settings = settings.Value;
            Client = new Mock<ISmtpClient>();
            Renderer = new Mock<ITemplateRenderService>();
            _service = new EmailService(settings, Client.Object, Renderer.Object);
        }

        public async Task SendEmailAsync<T>(T template, MailboxAddress to, CancellationToken cancellation = default)
             where T : EmailTemplate
        {
            Client.Setup(x => x.ConnectAsync(Settings.Host, Settings.Port, It.IsAny<SecureSocketOptions>(), cancellation)).Returns(Task.CompletedTask);
            Client.Setup(x => x.AuthenticateAsync(new NetworkCredential(Settings.Username, Settings.Password), cancellation)).Returns(Task.CompletedTask);
            Client.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), cancellation, null)).ReturnsAsync(string.Empty);

            Client.SetupGet(x => x.IsConnected).Returns(true);
            Client.Setup(x => x.DisconnectAsync(true, cancellation)).Returns(Task.CompletedTask);

            Renderer.Setup(x => x.RenderAsync(template)).ReturnsAsync(string.Empty);

            await _service.SendEmailAsync(template, to, cancellation);
        }

        public async Task SendEmailAsync<T>(T template, IEnumerable<MailboxAddress> to, CancellationToken cancellation = default)
             where T : EmailTemplate
        {
            Client.Setup(x => x.ConnectAsync(Settings.Host, Settings.Port, It.IsAny<SecureSocketOptions>(), cancellation)).Returns(Task.CompletedTask);
            Client.Setup(x => x.AuthenticateAsync(new NetworkCredential(Settings.Username, Settings.Password), cancellation)).Returns(Task.CompletedTask);
            Client.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), cancellation, null)).ReturnsAsync(string.Empty);

            Client.SetupGet(x => x.IsConnected).Returns(true);
            Client.Setup(x => x.DisconnectAsync(true, cancellation)).Returns(Task.CompletedTask);

            Renderer.Setup(x => x.RenderAsync(template)).ReturnsAsync(string.Empty);

            await _service.SendEmailAsync(template, to, cancellation);
        }
    }
}
