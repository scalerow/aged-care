using System.ComponentModel;

namespace Giantnodes.Service.Identity.Abstractions.Tenants.Commands
{
    public enum AcceptTenantInvitationCommandRejection
    {
        [Description("The invite cannot be found")]
        NotFound,

        [Description("The invite has expired")]
        Expired,
    }
}
