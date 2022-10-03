using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Tenants.Abstractions.Invitations.Requests
{
    public record SendTenantInviteRequestRejected : IRejected<SendTenantInviteRequestRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public SendTenantInviteRequestRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
