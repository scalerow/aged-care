using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record CreateUserCommandRejected : IRejected<CreateUserCommandRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public CreateUserCommandRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
