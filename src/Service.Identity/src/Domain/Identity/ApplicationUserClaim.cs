using Giantnodes.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Giantnodes.Service.Identity.Domain.Identity
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>, ITimestampableEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
