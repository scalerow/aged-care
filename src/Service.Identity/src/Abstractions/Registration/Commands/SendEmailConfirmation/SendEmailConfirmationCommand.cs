namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record SendEmailConfirmationCommand
    {
        public string Email { get; init; } = null!;
    }
}
