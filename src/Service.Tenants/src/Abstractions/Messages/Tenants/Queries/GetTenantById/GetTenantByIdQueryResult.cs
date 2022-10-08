using Giantnodes.Service.Tenants.Abstractions.Views.Tenants;

namespace Giantnodes.Service.Tenants.Abstractions.Messages.Tenants.Queries
{
    public record GetTenantByIdQueryResult
    {
        public TenantView Tenant { get; init; } = null!;
    }
}
