using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record SendUserEmailConfirmationCommandRejected : IRejected<SendUserEmailConfirmationCommandRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public SendUserEmailConfirmationCommandRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
