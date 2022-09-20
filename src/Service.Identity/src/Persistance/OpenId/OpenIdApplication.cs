using Giantnodes.Infrastructure.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Giantnodes.Service.Identity.Persistance.OpenId
{
    public class OpenIdApplication : OpenIddictEntityFrameworkCoreApplication<Guid, OpenIdAuthorization, OpenIdToken>, ITimestampableEntity
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
