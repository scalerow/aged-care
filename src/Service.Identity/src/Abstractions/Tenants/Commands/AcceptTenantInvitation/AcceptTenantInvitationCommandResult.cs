namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public record AcceptTenantInvitationCommandResult
    {
        public Guid UserTenantId { get; init; }
    }
}
