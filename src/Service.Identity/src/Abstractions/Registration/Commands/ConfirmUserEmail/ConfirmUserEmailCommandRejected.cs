using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record ConfirmUserEmailCommandRejected : IRejected<ConfirmUserEmailCommandRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public ConfirmUserEmailCommandRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
