namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public record ConfirmEmailRequestResult
    {
        public Guid UserId { get; init; }

        public string Email { get; init; } = string.Empty;
    }
}
