using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public enum CreateUserRequestRejection
    {
        [Description("The password provided is too weak")]
        PASSWORD_TOO_WEAK,

        [Description("The email is already associated with an account")]
        DUPLICATE_EMAIL,

        [Description("The identity provider retured an error response")]
        IDENTITY_ERROR,
    }
}
