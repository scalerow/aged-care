using System.ComponentModel;

namespace Giantnodes.Service.Tenants.Abstractions.Invitations.Requests
{
    public enum SendTenantInviteRequestRejection
    {
        [Description("The tenant cannot be found")]
        TenantNotFound,

        [Description("The user cannot be found")]
        UserNotFound,

        [Description("The user is already apart of the tenant")]
        AlreadyJoined,

        [Description("An invitation has already been sent recently")]
        AlreadySent
    }
}
