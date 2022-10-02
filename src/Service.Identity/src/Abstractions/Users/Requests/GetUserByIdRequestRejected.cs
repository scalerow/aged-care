using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Identity.Abstractions.Users.Requests
{
    public record GetUserByIdRequestRejected : IRejected<GetUserByIdRequestRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public GetUserByIdRequestRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
