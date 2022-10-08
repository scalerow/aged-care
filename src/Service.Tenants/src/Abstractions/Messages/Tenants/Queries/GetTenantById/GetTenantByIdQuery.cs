namespace Giantnodes.Service.Tenants.Abstractions.Messages.Tenants.Queries
{
    public record GetTenantByIdQuery
    {
        public Guid TenantId { get; init; }
    }
}
