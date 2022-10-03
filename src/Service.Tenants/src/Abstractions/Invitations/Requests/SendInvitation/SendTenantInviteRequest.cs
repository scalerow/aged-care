namespace Giantnodes.Service.Tenants.Abstractions.Invitations.Requests
{
    public record SendTenantInviteRequest
    {
        public Guid TenantId { get; init; }

        public Guid UserId { get; init; }
    }
}
