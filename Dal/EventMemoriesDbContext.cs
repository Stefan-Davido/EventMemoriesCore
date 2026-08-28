using DalEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using SharedItems;

namespace Dal
{
    public class EventMemoriesDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {       
        private readonly TenantProvider _tenantProvider;
        private readonly int _tenantId;

        public EventMemoriesDbContext(DbContextOptions<EventMemoriesDbContext> options, TenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
            _tenantId = _tenantProvider.GetTenantId();
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Post> Posts { get; set; }      
        public DbSet<Info> Infos { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations from all loaded assemblies so changes
            // in any class implementing IEntityTypeConfiguration<T> are picked up
            // when creating migrations.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .ToArray();

            foreach (var assembly in assemblies)
            {
                try
                {
                    modelBuilder.ApplyConfigurationsFromAssembly(assembly);
                }
                catch
                {
                    // Ignore assemblies that cannot be scanned for configurations
                }
            }

            // Apply query filter for soft delete to all entities implementing IIsDeleted
            ApplySoftDeleteFilter(modelBuilder);

        }

        private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            var softDeleteInterfaces = typeof(IIsDeleted);

            var softDeleteEntityTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(et => softDeleteInterfaces.IsAssignableFrom(et.ClrType));
         

            foreach (var entityType in softDeleteEntityTypes)
            {
                var method = typeof(EventMemoriesDbContext)
                    .GetMethod(nameof(ConfigureSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
           
            var tenantInterfaces = typeof(ITenantId);
            var tenantEntityTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(et => tenantInterfaces.IsAssignableFrom(et.ClrType));

            foreach (var entityType in tenantEntityTypes)
            {
                var method = typeof(EventMemoriesDbContext)
                    .GetMethod(nameof(ConfigureTenantFilter), BindingFlags.NonPublic)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
        }

        private static void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, IIsDeleted
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => !e.IsDeleted);
        }
        
        private void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, ITenantId
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => e.TenantId == _tenantId);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<ITenantId>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.TenantId).CurrentValue = _tenantId;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantId>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.TenantId).CurrentValue = _tenantId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

