using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public record ConfirmEmailRequestRejected : IRejected<ConfirmEmailRequestRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public ConfirmEmailRequestRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
