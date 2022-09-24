namespace Giantnodes.Service.Identity.Abstractions.Registration.Events
{
    public record UserCreatedEvent
    {
        public Guid UserId { get; init; }
    }
}
