using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public record SendTenantInvitationCommandRejected : IRejected<SendTenantInvitationCommandRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public SendTenantInvitationCommandRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
