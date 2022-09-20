using Giantnodes.Infrastructure.Abstractions;
using Giantnodes.Service.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Giantnodes.Service.Identity.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, ITimestampableEntity
    {
        [PersonalData]
        public string GivenName { get; set; } = null!;

        [PersonalData]
        public string FamilyName { get; set; } = null!;

        [PersonalData]
        public Gender Gender { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual string FullName => $"{GivenName} {FamilyName}";
    }
}
