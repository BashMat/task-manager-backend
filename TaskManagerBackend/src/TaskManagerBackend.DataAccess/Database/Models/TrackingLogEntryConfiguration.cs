using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagerBackend.DataAccess.Database.Models;

public class TrackingLogEntryConfiguration : IEntityTypeConfiguration<TrackingLogEntry>
{
    public void Configure(EntityTypeBuilder<TrackingLogEntry> builder)
    {
        builder.HasKey(e => e.Id).HasName("TrackingLogEntry_PK");

        builder.ToTable("TrackingLogEntry");

        builder.Property(e => e.Description).HasMaxLength(512);
        builder.Property(e => e.OrderIndex).HasColumnType("decimal(19, 2)");
        builder.Property(e => e.Title).HasMaxLength(256);

        builder.HasOne(d => d.CreatedByNavigation)
               .WithMany(p => p.TrackingLogEntryCreatedByNavigations)
               .HasForeignKey(d => d.CreatedBy)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntry_CreatedBy_FK");

        builder.HasOne(d => d.TrackingLogEntryStatus)
               .WithMany(p => p.TrackingLogEntries)
               .HasForeignKey(d => d.StatusId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntry_StatusId_FK");

        builder.HasOne(d => d.TrackingLog)
               .WithMany(p => p.TrackingLogEntries)
               .HasForeignKey(d => d.TrackingLogId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntry_TrackingLogId_FK");

        builder.HasOne(d => d.UpdatedByNavigation)
               .WithMany(p => p.TrackingLogEntryUpdatedByNavigations)
               .HasForeignKey(d => d.UpdatedBy)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("TrackingLogEntry_UpdatedBy_FK");
    }
}