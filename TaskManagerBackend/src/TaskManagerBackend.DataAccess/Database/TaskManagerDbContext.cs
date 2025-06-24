#region Usings

using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database.Models;

#endregion

namespace TaskManagerBackend.DataAccess.Database;

public partial class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext()
    {
    }

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TrackingLogEntryStatus> TrackingLogEntryStatuses { get; set; }

    public virtual DbSet<TrackingLog> TrackingLogs { get; set; }

    public virtual DbSet<TrackingLogEntry> TrackingLogEntries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackingLogEntryStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TrackingLogEntryStatus__3214EC07A5C436E6");

            entity.ToTable("TrackingLogEntryStatus");

            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TrackingLogEntryStatusCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntryStatus_CreatedBy_FK");

            entity.HasOne(d => d.TrackingLog).WithMany(p => p.TrackingLogEntryStatuses)
                .HasForeignKey(d => d.TrackingLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntryStatus_TrackingLogId_FK");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TrackingLogEntryStatusUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntryStatus_UpdatedBy_FK");
        });

        modelBuilder.Entity<TrackingLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tracking__3214EC07683BD94B");

            entity.ToTable("TrackingLog");

            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TrackingLogCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLog_CreatedBy_FK");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TrackingLogUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLog_UpdatedBy_FK");
        });

        modelBuilder.Entity<TrackingLogEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tracking__3214EC07628C810A");

            entity.ToTable("TrackingLogEntry");

            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.OrderIndex).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TrackingLogEntryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntry_CreatedBy_FK");

            entity.HasOne(d => d.TrackingLogEntryStatus).WithMany(p => p.TrackingLogEntries)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntry_StatusId_FK");

            entity.HasOne(d => d.TrackingLog).WithMany(p => p.TrackingLogEntries)
                .HasForeignKey(d => d.TrackingLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntry_TrackingLogId_FK");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TrackingLogEntryUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrackingLogEntry_UpdatedBy_FK");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07AA793C03");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(256);
            entity.Property(e => e.LastName).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
