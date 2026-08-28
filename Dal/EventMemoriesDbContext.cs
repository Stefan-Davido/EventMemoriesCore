using DalEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Dal
{
    public class EventMemoriesDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public EventMemoriesDbContext(DbContextOptions<EventMemoriesDbContext> options) : base(options)
        {
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
            var entityTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(et => softDeleteInterfaces.IsAssignableFrom(et.ClrType));

            foreach (var entityType in entityTypes)
            {
                var method = typeof(EventMemoriesDbContext)
                    .GetMethod(nameof(ConfigureSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
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
    }
}

