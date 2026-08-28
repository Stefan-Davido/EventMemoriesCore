using DalEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dal.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.CreatedTime)
                .IsRequired();

            builder.Property(e => e.EventDate)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.HasOne(e => e.Owner)
                .WithMany(u => u.OwnedEvents)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Tenant)
                .WithMany(t => t.Events)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Posts)
                .WithOne(p => p.Event)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Infos)
                .WithOne(i => i.Event)
                .HasForeignKey(i => i.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Configurations)
                .WithOne(c => c.Event)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.Id)
                .IsUnique();
        }
    }
}
