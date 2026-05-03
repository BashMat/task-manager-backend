using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagerBackend.DataAccess.Database.Models;

public class TrackingLogEntryStatusConfiguration : IEntityTypeConfiguration<TrackingLogEntryStatus>
{
    public void Configure(EntityTypeBuilder<TrackingLogEntryStatus> builder)
    {
        builder.HasKey(e => e.Id).HasName("TrackingLogEntryStatus_PK");

        builder.ToTable("TrackingLogEntryStatus");

        builder.Property(e => e.Description).HasMaxLength(512);
        builder.Property(e => e.Title).HasMaxLength(256);

        builder.HasOne(d => d.CreatedByNavigation)
               .WithMany(p => p.TrackingLogEntryStatusCreatedByNavigations)
               .HasForeignKey(d => d.CreatedBy)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntryStatus_CreatedBy_FK");

        builder.HasOne(d => d.TrackingLog)
               .WithMany(p => p.TrackingLogEntryStatuses)
               .HasForeignKey(d => d.TrackingLogId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntryStatus_TrackingLogId_FK");

        builder.HasOne(d => d.UpdatedByNavigation)
               .WithMany(p => p.TrackingLogEntryStatusUpdatedByNavigations)
               .HasForeignKey(d => d.UpdatedBy)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntryStatus_UpdatedBy_FK");
    }
}