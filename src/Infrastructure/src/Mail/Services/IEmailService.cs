using Giantnodes.Infrastructure.Mail.Abstractions;
using MimeKit;

namespace Giantnodes.Infrastructure.Mail.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync<T>(T temaplte, MailboxAddress to, CancellationToken cancellation = default)
            where T : EmailTemplate;

        Task SendEmailAsync<T>(T temaplte, IEnumerable<MailboxAddress> to, CancellationToken cancellation = default)
            where T : EmailTemplate;
    }
}
