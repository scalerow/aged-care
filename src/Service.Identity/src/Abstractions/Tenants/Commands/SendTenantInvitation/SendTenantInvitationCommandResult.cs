namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public record SendTenantInvitationCommandResult
    {
        public Guid UserTenantInviteId { get; init; }
    }
}
