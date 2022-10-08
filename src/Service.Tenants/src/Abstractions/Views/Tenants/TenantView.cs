using Giantnodes.Service.Tenants.Abstractions.Views.Tenants.Partials;

namespace Giantnodes.Service.Tenants.Abstractions.Views.Tenants
{
    public record TenantView : ITenant
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;
    }
}
