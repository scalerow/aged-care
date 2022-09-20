using Giantnodes.Infrastructure.Abstractions;
using Giantnodes.Service.Identity.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Identity.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            NullifyEmptyStrings();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            AddTimestamps();
            NullifyEmptyStrings();
            return base.SaveChangesAsync(cancellation);
        }

        private void AddTimestamps()
        {
            foreach (var entry in ChangeTracker.Entries<ITimestampableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        private void NullifyEmptyStrings()
        {
            foreach (var entity in ChangeTracker.Entries())
            {
                var properties = entity
                    .Entity
                    .GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

                foreach (var property in properties)
                {
                    if (string.IsNullOrWhiteSpace(property.GetValue(entity.Entity) as string))
                        property.SetValue(entity.Entity, null, null);
                }
            }
        }
    }
}
