using Giantnodes.Service.Identity.Domain.Identity;

namespace Giantnodes.Service.Identity.Domain.Entities
{
    public class UserTenant
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public Guid TenantId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
