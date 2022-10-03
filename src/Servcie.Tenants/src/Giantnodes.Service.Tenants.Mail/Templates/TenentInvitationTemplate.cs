using Giantnodes.Infrastructure.Mail.Abstractions;
using Giantnodes.Service.Tenants.Domain.Entities;
using MimeKit;

namespace Giantnodes.Service.Tenants.Mail.Templates
{
    public class TenentInvitationTemplate : EmailTemplate
    {
        public override string Path => "~/Views/TenentInvitation.cshtml";

        public override TemplateRenderEngine Engine => TemplateRenderEngine.Mjml;

        public override string Subject => $"{GivenName} invited you to {Tenant.Name}";

        public override MailboxAddress From => new MailboxAddress("Giantnodes", "noreply@giantnodes.com");

        public Tenant Tenant { get; set; } = null!;

        public Guid Code { get; set; }

        public string GivenName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
