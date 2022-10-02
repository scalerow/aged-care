namespace Giantnodes.Service.Identity.Abstractions.Users.Requests
{
    public record GetUserByIdRequestResult
    {
        public Guid UserId { get; init; }

        public string Email { get; init; } = string.Empty;

        public string GivenName { get; init; } = string.Empty;

        public string FamilyName { get; init; } = string.Empty;
    }
}
