using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Service.Identity.Domain.Identity;
using MimeKit;

namespace Giantnodes.Service.Identity.Mail.Templates
{
    public class EmailConfirmationTemplate : EmailTemplate
    {
        public override string Path => "~/Views/EmailConfirmation.cshtml";

        public override TemplateRenderEngine Engine => TemplateRenderEngine.Mjml;

        public override string Subject => "Email Verification";

        public override MailboxAddress From => new MailboxAddress("Giantnodes", "noreply@giantnodes.com");

        public ApplicationUser User { get; init; } = null!;

        public string Code { get; init; } = string.Empty;

        public string VerifyLink = string.Empty;
    }
}
