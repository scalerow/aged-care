namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record CreateUserCommand
    {
        public string Email { get; init; } = null!;

        public string GivenName { get; init; } = null!;

        public string FamilyName { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}
