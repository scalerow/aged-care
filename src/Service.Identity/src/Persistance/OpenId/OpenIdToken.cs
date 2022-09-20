using Giantnodes.Infrastructure.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Giantnodes.Service.Identity.Persistance.OpenId
{
    public class OpenIdToken : OpenIddictEntityFrameworkCoreToken<Guid, OpenIdApplication, OpenIdAuthorization>, ITimestampableEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
