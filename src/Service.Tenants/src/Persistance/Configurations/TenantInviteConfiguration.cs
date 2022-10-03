using Giantnodes.Service.Tenants.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Tenants.Persistance.Configurations
{
    public class TenantInviteConfiguration : IEntityTypeConfiguration<TenantInvite>
    {
        public void Configure(EntityTypeBuilder<TenantInvite> builder)
        {
            builder.HasIndex(p => new { p.TenantId, p.UserId })
                .IsUnique();
        }
    }
}
