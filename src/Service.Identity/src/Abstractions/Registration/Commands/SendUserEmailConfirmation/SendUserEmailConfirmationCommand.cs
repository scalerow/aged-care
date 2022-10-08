namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record SendUserEmailConfirmationCommand
    {
        public string Email { get; init; } = null!;
    }
}
