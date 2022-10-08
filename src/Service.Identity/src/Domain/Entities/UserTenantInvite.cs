using Giantnodes.Infrastructure.Abstractions;
using Giantnodes.Service.Identity.Domain.Identity;

namespace Giantnodes.Service.Identity.Domain.Entities
{
    public class UserTenantInvite : ITimestampableEntity, IExpirableEntity
    {
        /// <summary>
        /// The duration of the invitation until it is deemed as expired.
        /// </summary>
        public static readonly int LifetimeHours = 48;

        /// <summary>
        /// The duration until an existing token can be resent to the users email.
        /// </summary>
        public static readonly int ResendMinutes = 5;

        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public Guid TenantId { get; set; }

        public Guid Token { get; set; }

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(LifetimeHours);

        public DateTime? LastSentAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
