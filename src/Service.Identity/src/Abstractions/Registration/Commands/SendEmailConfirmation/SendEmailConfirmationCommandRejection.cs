using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Registration.Commands
{
    public enum SendEmailConfirmationCommandRejection
    {
        [Description("The user associated with the email cannot be found")]
        NOT_FOUND,

        [Description("The email has already been confirmed")]
        ALREADY_CONFIRMED,
    }
}
