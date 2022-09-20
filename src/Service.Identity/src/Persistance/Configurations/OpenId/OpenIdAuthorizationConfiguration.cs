using Giantnodes.Service.Identity.Persistance.OpenId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Identity.Persistance.Configurations
{
    public class OpenIdAuthorizationConfiguration : IEntityTypeConfiguration<OpenIdAuthorization>
    {
        public void Configure(EntityTypeBuilder<OpenIdAuthorization> builder)
        {
            builder.ToTable("open_id_authorizations");
        }
    }
}
