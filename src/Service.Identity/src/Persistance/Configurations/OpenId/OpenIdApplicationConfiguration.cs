using Giantnodes.Service.Identity.Persistance.OpenId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Identity.Persistance.Configurations
{
    public class OpenIdApplicationConfiguration : IEntityTypeConfiguration<OpenIdApplication>
    {
        public void Configure(EntityTypeBuilder<OpenIdApplication> builder)
        {
            builder.ToTable("open_id_applications");
        }
    }
}
