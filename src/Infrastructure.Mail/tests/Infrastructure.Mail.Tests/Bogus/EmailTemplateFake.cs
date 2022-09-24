using Bogus;
using Giantnodes.Infrastructure.Mail.Abstractions;
using MimeKit;

namespace Giantnodes.Infrastructure.Mail.Tests.Bogus
{
    public class EmailTemplateFake : EmailTemplate
    {
        public override string Path => new Faker().System.FilePath();

        public override TemplateRenderEngine Engine => TemplateRenderEngine.Razor;

        public override string Subject => new Faker().Lorem.Sentence();

        public override MailboxAddress From => new MailboxAddressFaker().Generate();
    }
}
