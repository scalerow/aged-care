using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record SendEmailConfirmationCommandRejected : IRejected<SendEmailConfirmationCommandRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public SendEmailConfirmationCommandRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
