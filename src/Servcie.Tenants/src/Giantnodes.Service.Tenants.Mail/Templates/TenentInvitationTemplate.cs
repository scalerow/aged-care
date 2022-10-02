using Giantnodes.Infrastructure.Mail.Abstractions;
using MimeKit;

namespace Giantnodes.Service.Identity.Mail.Templates
{
    public class TenentInvitationTemplate : EmailTemplate
    {
        public override string Path => "~/Views/TenantInvite.cshtml";

        public override TemplateRenderEngine Engine => TemplateRenderEngine.Mjml;

        public override string Subject => "Tenant Invitation";

        public override MailboxAddress From => new MailboxAddress("Giantnodes", "noreply@giantnodes.com");

        public Guid Code { get; set; }

        public string TenantName { get; set; } = string.Empty;

        public string UserFullName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;
    }
}
