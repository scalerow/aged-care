using Bogus;
using Giantnodes.Service.Identity.Domain.Enums;
using Giantnodes.Service.Identity.Domain.Identity;

namespace Giantnodes.Service.Identity.Shared.Tests.Bogus
{
    public class ApplicationUserFaker : Faker<ApplicationUser>
    {
        public ApplicationUserFaker()
        {
            RuleFor(p => p.Id, f => f.Random.Uuid());
            RuleFor(p => p.Email, f => f.Internet.Email());
            RuleFor(p => p.EmailConfirmed, f => f.Random.Bool());
            RuleFor(p => p.UserName, (f, u) => f.Internet.UserName(u.GivenName, u.FamilyName));
            RuleFor(p => p.GivenName, f => f.Name.FirstName());
            RuleFor(p => p.FamilyName, f => f.Name.LastName());
            RuleFor(p => p.Gender, f => f.Random.Enum<Gender>());
            RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber());
            RuleFor(p => p.PhoneNumberConfirmed, f => f.Random.Bool());
            RuleFor(p => p.TwoFactorEnabled, f => f.Random.Bool());
        }
    }
}
