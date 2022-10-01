namespace Giantnodes.Service.Identity.Abstractions.Registration.Events
{
    public record UserConfirmedEmailEvent
    {
        public Guid UserId { get; init; }

        public string Email { get; init; } = string.Empty;
    }
}
