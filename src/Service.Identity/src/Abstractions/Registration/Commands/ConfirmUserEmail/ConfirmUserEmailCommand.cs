namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record ConfirmUserEmailCommand
    {
        public string Token { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;
    }
}
