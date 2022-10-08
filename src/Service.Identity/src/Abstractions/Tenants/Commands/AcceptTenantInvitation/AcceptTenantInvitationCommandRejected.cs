using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public record AcceptTenantInvitationCommandRejected : IRejected<AcceptTenantInvitationCommandRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public AcceptTenantInvitationCommandRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
