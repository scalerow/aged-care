using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Service.Identity.Domain.Identity;
using MimeKit;

namespace Giantnodes.Service.Identity.Mail.Templates
{
    public class TenentInvitationTemplate : EmailTemplate
    {
        public override string Path => "~/Views/TenentInvitation.cshtml";

        public override TemplateRenderEngine Engine => TemplateRenderEngine.Mjml;

        public override string Subject => $"{Recipient.FullName} invited you to {Tenant}";

        public override MailboxAddress From => new MailboxAddress("Giantnodes", "noreply@giantnodes.com");

        public ApplicationUser Recipient { get; init; } = null!;

        public string Tenant { get; init; } = string.Empty;

        public Guid Token { get; set; }
    }
}
