using Giantnodes.Infrastructure.EntityFramework.Converters;
using Giantnodes.Service.Identity.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Identity.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .Property(e => e.Gender)
                .HasConversion<string>();

            builder
                .Property(e => e.Email)
                .HasConversion<LowerCaseConverter>();

            builder
                .Property(e => e.GivenName)
                .HasConversion<AlphabeticalConverter>()
                .HasConversion<LowerCaseConverter>();

            builder
                .Property(e => e.FamilyName)
                .HasConversion<AlphabeticalConverter>()
                .HasConversion<LowerCaseConverter>();

            builder.ToTable("users");
        }
    }
}
