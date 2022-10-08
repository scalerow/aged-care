using Giantnodes.Infrastructure.MassTransit;

namespace Giantnodes.Service.Tenants.Abstractions.Messages.Tenants.Queries
{
    public record GetTenantByIdQueryRejected : IRejected<GetTenantByIdQueryRejection>
    {
        public Guid ConversationId { get; init; }

        public DateTime TimeStamp { get; init; }

        public GetTenantByIdQueryRejection ErrorCode { get; init; }

        public string Reason { get; init; } = null!;
    }
}
