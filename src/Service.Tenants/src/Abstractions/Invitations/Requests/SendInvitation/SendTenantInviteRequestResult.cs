namespace Giantnodes.Service.Tenants.Abstractions.Invitations.Requests
{
    public record SendTenantInviteRequestResult
    {
        public Guid TenantId { get; init; }

        public Guid UserId { get; init; }
    }
}
