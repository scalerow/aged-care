using Giantnodes.Infrastructure.Mail.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;

namespace Giantnodes.Infrastructure.Mail.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ISmtpClient _client;
        private readonly ITemplateRenderService _renderer;

        public EmailService(IOptions<EmailSettings> settings, ISmtpClient client, ITemplateRenderService renderer)
        {
            _settings = settings.Value;
            _client = client;
            _renderer = renderer;
        }

        public async Task SendEmailAsync<T>(T template, MailboxAddress to, CancellationToken cancellation = default)
            where T : EmailTemplate
        {
            var message = await BuildEmailContent(template, to);

            await SendAsync<T>(new MimeMessage[] { message }, cancellation);
        }

        public async Task SendEmailAsync<T>(T template, IEnumerable<MailboxAddress> to, CancellationToken cancellation = default)
            where T : EmailTemplate
        {
            var tasks = to
                .DistinctBy(email => email.Address)
                .Select(email => BuildEmailContent(template, email))
                .ToList();

            await Task.WhenAll(tasks);

            var messages = tasks.Select(task => task.Result);
            await SendAsync<T>(messages, cancellation);
        }

        private async Task SendAsync<T>(IEnumerable<MimeMessage> messages, CancellationToken cancellation = default)
        {
            try
            {
                await _client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellation);
                await _client.AuthenticateAsync(new NetworkCredential(_settings.Username, _settings.Password), cancellation);

                var tasks = messages.Select(message => _client.SendAsync(message, cancellation));
                await Task.WhenAll(tasks);
            }
            finally
            {
                if (_client.IsConnected)
                    await _client.DisconnectAsync(true, CancellationToken.None);
            }
        }

        private async Task<MimeMessage> BuildEmailContent<T>(T template, MailboxAddress to)
            where T : EmailTemplate
        {
            var message = new MimeMessage();
            message.Subject = template.Subject;
            message.From.Add(template.From);
            message.To.Add(to);

            var builder = new BodyBuilder();
            builder.HtmlBody = await _renderer.RenderAsync<T>(template);

            message.Body = builder.ToMessageBody();
            return message;
        }
    }
}
