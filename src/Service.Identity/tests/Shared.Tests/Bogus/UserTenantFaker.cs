using Bogus;
using Giantnodes.Service.Identity.Domain.Entities;
using Giantnodes.Service.Identity.Domain.Identity;

namespace Giantnodes.Service.Identity.Shared.Tests.Bogus
{
    public class UserTenantFaker : Faker<UserTenant>
    {
        public UserTenantFaker(ApplicationUser user)
        {
            RuleFor(p => p.Id, f => f.Random.Uuid());
            RuleFor(p => p.UserId, user.Id);
            RuleFor(p => p.TenantId, f => f.Random.Uuid());
            RuleFor(p => p.UpdatedAt, DateTime.UtcNow);
            RuleFor(p => p.CreatedAt, DateTime.UtcNow);
        }
    }
}
