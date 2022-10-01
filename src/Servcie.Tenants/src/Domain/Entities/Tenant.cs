using Giantnodes.Infrastructure.Abstractions;

namespace Giantnodes.Service.Tenants.Domain.Entities
{
    public class Tenant : ITimestampableEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
