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

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new TrackingLogEntryStatusConfiguration().Configure(modelBuilder.Entity<TrackingLogEntryStatus>());
        new TrackingLogConfiguration().Configure(modelBuilder.Entity<TrackingLog>());
        new TrackingLogEntryConfiguration().Configure(modelBuilder.Entity<TrackingLogEntry>());
        new RefreshTokenConfiguration().Configure(modelBuilder.Entity<RefreshToken>());
        new UserConfiguration().Configure(modelBuilder.Entity<User>());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
