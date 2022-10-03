using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public enum CreateUserRequestRejection
    {
        [Description("The password provided is too weak")]
        PasswordTooWeak,

        [Description("The email is already associated with an account")]
        DuplicateEmail,

        [Description("The identity provider retured an error response")]
        IdentityError,
    }
}
