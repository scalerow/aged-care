using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Requests
{
    public enum ConfirmEmailRequestRejection
    {
        [Description("The user associated with the token or email cannot be found")]
        NOT_FOUND,

        [Description("The email has already been confirmed")]
        ALREADY_CONFIRMED,

        [Description("The identity provider retured an error response")]
        IDENTITY_ERROR,
    }
}
