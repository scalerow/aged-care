using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Users.Requests
{
    public enum GetUserByIdRequestRejection
    {
        [Description("The user cannot be found")]
        NotFound
    }
}
