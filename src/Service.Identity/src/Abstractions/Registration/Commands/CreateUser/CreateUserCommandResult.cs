namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record CreateUserCommandResult
    {
        public Guid UserId { get; init; }
    }
}
