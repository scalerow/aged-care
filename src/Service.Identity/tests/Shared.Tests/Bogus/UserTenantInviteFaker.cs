using Bogus;
using Giantnodes.Service.Identity.Domain.Entities;
using Giantnodes.Service.Identity.Domain.Identity;

namespace Giantnodes.Service.Identity.Shared.Tests.Bogus
{
    public class UserTenantInviteFaker : Faker<UserTenantInvite>
    {
        public UserTenantInviteFaker(ApplicationUser user)
        {
            RuleFor(p => p.Id, f => f.Random.Uuid());
            RuleFor(p => p.UserId, user.Id);
            RuleFor(p => p.TenantId, f => f.Random.Uuid());
            RuleFor(p => p.Token, f => f.Random.Uuid());
            RuleFor(p => p.LastSentAt, f => f.Date.Past());
            RuleFor(p => p.ExpiresAt, f => f.Date.Future());
            RuleFor(p => p.UpdatedAt, DateTime.UtcNow);
            RuleFor(p => p.CreatedAt, DateTime.UtcNow);
        }
    }
}
