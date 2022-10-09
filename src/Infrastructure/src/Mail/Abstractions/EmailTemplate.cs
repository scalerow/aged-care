using MimeKit;

namespace Giantnodes.Infrastructure.Mail.Abstractions
{
    public abstract class EmailTemplate
    {
        public abstract string Path { get; }

        public abstract TemplateRenderEngine Engine { get; }

        public abstract string Subject { get; }

        public abstract MailboxAddress From { get; }
    }
}
