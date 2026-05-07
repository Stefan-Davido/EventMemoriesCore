using DalEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dal.Configurations
{
    public class InfoConfiguration : IEntityTypeConfiguration<Info>
    {
        public void Configure(EntityTypeBuilder<Info> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Text)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(i => i.Date)
                .IsRequired(false);

            builder.HasOne(i => i.Event)
                .WithMany(e => e.Infos)
                .HasForeignKey(i => i.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.User)
                .WithMany(u => u.Infos)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
