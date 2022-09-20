using Giantnodes.Service.Identity.Persistance.OpenId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Identity.Persistance.Configurations
{
    public class OpenIdScopeConfiguration : IEntityTypeConfiguration<OpenIdScope>
    {
        public void Configure(EntityTypeBuilder<OpenIdScope> builder)
        {
            builder.ToTable("open_id_scopes");
        }
    }
}
