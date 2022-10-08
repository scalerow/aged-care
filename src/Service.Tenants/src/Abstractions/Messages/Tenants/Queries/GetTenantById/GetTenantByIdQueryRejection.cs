using System.ComponentModel;

namespace Giantnodes.Service.Tenants.Abstractions.Messages.Tenants.Queries
{
    public enum GetTenantByIdQueryRejection
    {
        [Description("The tenant cannot be found")]
        NotFound,
    }
}
