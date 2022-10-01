namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public record ConfirmEmailRequest
    {
        public string Token { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;
    }
}
