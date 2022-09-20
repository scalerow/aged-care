using Giantnodes.Infrastructure.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Giantnodes.Service.Identity.Persistance.OpenId
{
    public class OpenIdScope : OpenIddictEntityFrameworkCoreScope<Guid>, ITimestampableEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
