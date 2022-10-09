using Bogus;
using MimeKit;

namespace Giantnodes.Infrastructure.Mail.Tests.Bogus
{
    public class MailboxAddressFaker : Faker<MailboxAddress>
    {
        public MailboxAddressFaker()
        {
            CustomInstantiator(f => new MailboxAddress(f.Name.FullName(), f.Internet.Email()));
        }
    }
}
