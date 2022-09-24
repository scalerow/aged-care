using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public record CreateUserRequestRejected : IRejected<CreateUserRequestRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public CreateUserRequestRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
