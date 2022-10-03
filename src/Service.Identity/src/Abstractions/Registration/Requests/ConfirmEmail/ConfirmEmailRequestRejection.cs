using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public enum ConfirmEmailRequestRejection
    {
        [Description("The user associated with the token or email cannot be found")]
        NotFound,

        [Description("The email has already been confirmed")]
        AlreadyConfirmed,

        [Description("The identity provider retured an error response")]
        IdentityError,
    }
}
