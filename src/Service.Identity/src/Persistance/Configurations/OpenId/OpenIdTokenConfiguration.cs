using Giantnodes.Service.Identity.Persistance.OpenId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Identity.Persistance.Configurations
{
    public class OpenIdTokenConfiguration : IEntityTypeConfiguration<OpenIdToken>
    {
        public void Configure(EntityTypeBuilder<OpenIdToken> builder)
        {
            builder.ToTable("open_id_tokens");
        }
    }
}
