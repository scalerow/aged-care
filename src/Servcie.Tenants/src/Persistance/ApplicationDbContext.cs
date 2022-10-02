using Giantnodes.Infrastructure.Abstractions;
using Giantnodes.Service.Tenants.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Tenants.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
        public DbSet<TenantInvite> TenantInvites => Set<TenantInvite>();

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
