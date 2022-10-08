namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public record ConfirmUserEmailCommandResult
    {
        public Guid UserId { get; init; }
    }
}
