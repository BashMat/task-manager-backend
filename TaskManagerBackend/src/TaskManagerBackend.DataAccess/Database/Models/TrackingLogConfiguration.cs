using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagerBackend.DataAccess.Database.Models;

public class TrackingLogConfiguration : IEntityTypeConfiguration<TrackingLog>
{
    public void Configure(EntityTypeBuilder<TrackingLog> builder)
    {
        builder.HasKey(e => e.Id).HasName("TrackingLog_PK");

        builder.ToTable("TrackingLog");

        builder.Property(e => e.Description).HasMaxLength(512);
        builder.Property(e => e.Title).HasMaxLength(256);

        builder.HasOne(d => d.CreatedByNavigation)
               .WithMany(p => p.TrackingLogCreatedByNavigations)
               .HasForeignKey(d => d.CreatedBy)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLog_CreatedBy_FK");

        builder.HasOne(d => d.UpdatedByNavigation)
               .WithMany(p => p.TrackingLogUpdatedByNavigations)
               .HasForeignKey(d => d.UpdatedBy)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLog_UpdatedBy_FK");
    }
}