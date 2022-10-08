using Giantnodes.Service.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Identity.Persistance.Configurations
{
    public class UserTenantInviteConfiguration : IEntityTypeConfiguration<UserTenantInvite>
    {
        public void Configure(EntityTypeBuilder<UserTenantInvite> builder)
        {
            builder.HasIndex(p => new { p.UserId, p.TenantId })
                .IsUnique();

            builder.HasIndex(p => p.Token)
                .IsUnique();
        }
    }
}
