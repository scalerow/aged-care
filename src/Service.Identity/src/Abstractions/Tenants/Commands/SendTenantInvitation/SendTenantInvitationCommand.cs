namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public record SendTenantInvitationCommand
    {
        public Guid TenantId { get; init; }

        public Guid UserId { get; init; }
    }
}
