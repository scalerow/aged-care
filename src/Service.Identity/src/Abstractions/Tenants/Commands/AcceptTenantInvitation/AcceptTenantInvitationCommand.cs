namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public record AcceptTenantInvitationCommand
    {
        public Guid Token { get; init; }
    }
}
