using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public enum SendTenantInvitationCommandRejection
    {
        [Description("The tenant cannot be found")]
        TenantNotFound,

        [Description("The user cannot be found")]
        UserNotFound,

        [Description("The user is already present in the tenant")]
        UserAlreadyPresent,

        [Description("An invitation has already been sent recently")]
        AlreadySent
    }
}
