namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public record CreateUserRequestResult
    {
        public Guid UserId { get; init; }
    }
}
