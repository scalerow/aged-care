using Giantnodes.Infrastructure.Abstractions;

namespace Giantnodes.Service.Tenants.Domain.Entities
{
    public class TenantUser : ITimestampableEntity
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public virtual Tenant? Tenant { get; set; }

        public Guid UserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
