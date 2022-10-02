using Giantnodes.Service.Tenants.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Tenants.Persistance.Configurations
{
    public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
    {
        public void Configure(EntityTypeBuilder<TenantUser> builder)
        {
            builder.HasIndex(p => new { p.TenantId, p.UserId })
                .IsUnique();
        }
    }
}
