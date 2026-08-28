using DalEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dal.Configurations
{
    public class ConfigurationConfiguration : IEntityTypeConfiguration<Configuration>
    {
        public void Configure(EntityTypeBuilder<Configuration> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(c => c.Event)
                .WithMany(e => e.Configurations)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
