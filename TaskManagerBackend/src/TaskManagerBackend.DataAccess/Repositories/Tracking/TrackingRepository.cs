#region Usings

using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Domain.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public class TrackingRepository(TaskManagerDbContext dbContext) : ITrackingRepository
{
    #region Tracking Log

    public async Task<Domain.Tracking.TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog logToInsert, 
                                                                                  CancellationToken cancellationToken)
    {
        TrackingLog trackingLog = new()
                                  {
                                      Title = logToInsert.Title,
                                      Description = logToInsert.Description,
                                      CreatedBy = logToInsert.CreatedById,
                                      CreatedAt = logToInsert.CreatedAt,
                                      UpdatedBy = logToInsert.CreatedById,
                                      UpdatedAt = logToInsert.CreatedAt
                                  };
        dbContext.TrackingLogs.Add(trackingLog);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetTrackingLogById(trackingLog.Id, cancellationToken);
    }

    public async Task<List<Domain.Tracking.TrackingLog.TrackingLog>> GetAllTrackingLogs(int userId, 
                                                                                        CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogs.AsNoTracking()
                              .FilterByCreator(userId)
                              .SelectEagerly()
                              .Select(log => log.ToDomain())
                              .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Tracking.TrackingLog.TrackingLog?> GetTrackingLogById(int trackingLogId, 
                                                                                   CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogs.AsNoTracking()
                              .FilterById(trackingLogId)
                              .SelectEagerly()
                              .Select(log => log.ToDomain())
                              .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Domain.Tracking.TrackingLog.TrackingLog>> DeleteTrackingLogById(int userId,
                                                                                           int trackingLogId,
                                                                                           CancellationToken cancellationToken)
    {
        int deletedCount = await dbContext.TrackingLogs.FilterById(trackingLogId).ExecuteDeleteAsync(cancellationToken);

        if (deletedCount == 0)
        {
            return new List<Domain.Tracking.TrackingLog.TrackingLog>();
        }

        return await GetAllTrackingLogs(userId, cancellationToken);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry logEntryToInsert, 
                                                                                                 CancellationToken cancellationToken)
    {
        TrackingLogEntry entry = new()
                                 {
                                     Title = logEntryToInsert.Title,
                                     Description = logEntryToInsert.Description,
                                     TrackingLogId = logEntryToInsert.TrackingLogId,
                                     StatusId = logEntryToInsert.StatusId,
                                     Priority = logEntryToInsert.Priority,
                                     OrderIndex = (decimal) logEntryToInsert.OrderIndex,
                                     CreatedBy = logEntryToInsert.CreatedById,
                                     CreatedAt = logEntryToInsert.CreatedAt,
                                     UpdatedBy = logEntryToInsert.CreatedById,
                                     UpdatedAt = logEntryToInsert.CreatedAt
                                 };
        dbContext.TrackingLogEntries.Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetTrackingLogEntryById(entry.Id, cancellationToken);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntry.TrackingLogEntry>> GetAllTrackingLogEntries(int userId,
                                                                                                        CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogEntries.AsNoTracking()
                              .FilterByCreator(userId)
                              .Include(entry => entry.CreatedByNavigation)
                              .Include(entry => entry.UpdatedByNavigation)
                              .Include(entry => entry.TrackingLogEntryStatus)
                              .Select(entry => entry.ToDomain())
                              .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId,
                                                                                                  CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogEntries.AsNoTracking()
                              .FilterById(trackingLogEntryId)
                              .Include(entry => entry.CreatedByNavigation)
                              .Include(entry => entry.UpdatedByNavigation)
                              .Include(entry => entry.TrackingLogEntryStatus)
                              .Select(entry => entry.ToDomain())
                              .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> UpdateTrackingLogEntryById(int trackingLogEntryId, 
                                                                                                     UpdatableTrackingLogEntry updatableTrackingLogEntry,
                                                                                                     CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await dbContext.TrackingLogEntries.FilterById(trackingLogEntryId)
                                                 .FirstOrDefaultAsync(cancellationToken);

        if (entry is null)
        {
            return null;
        }

        entry.Title = updatableTrackingLogEntry.Title;
        entry.Description = updatableTrackingLogEntry.Description;
        entry.TrackingLogId = updatableTrackingLogEntry.TrackingLogId;
        entry.StatusId = updatableTrackingLogEntry.StatusId;
        entry.Priority = updatableTrackingLogEntry.Priority;
        entry.OrderIndex = (decimal)updatableTrackingLogEntry.OrderIndex;
        entry.UpdatedBy = updatableTrackingLogEntry.UpdatedBy;
        entry.UpdatedAt = updatableTrackingLogEntry.UpdatedAt;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetTrackingLogEntryById(trackingLogEntryId, cancellationToken);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntry.TrackingLogEntry>> DeleteTrackingLogEntryById(int userId, 
                                                                                                          int trackingLogEntryId,
                                                                                                          CancellationToken cancellationToken)
    {
        int deletedCount = await dbContext.TrackingLogEntries.FilterById(trackingLogEntryId)
                                          .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount == 0)
        {
            return new List<Domain.Tracking.TrackingLogEntry.TrackingLogEntry>();
        }

        return await GetAllTrackingLogEntries(userId, cancellationToken);
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert,
                                                                                                                   CancellationToken cancellationToken)
    {
        TrackingLogEntryStatus trackingLogEntryStatus = new()
                        {
                            Title = statusToInsert.Title,
                            Description = statusToInsert.Description,
                            TrackingLogId = statusToInsert.TrackingLogId,
                            CreatedBy = statusToInsert.CreatedById,
                            CreatedAt = statusToInsert.CreatedAt,
                            UpdatedBy = statusToInsert.CreatedById,
                            UpdatedAt = statusToInsert.CreatedAt
                        };
        dbContext.TrackingLogEntryStatuses.Add(trackingLogEntryStatus);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus(trackingLogEntryStatus.Id,
                                                          trackingLogEntryStatus.Title,
                                                          trackingLogEntryStatus.Description,
                                                          trackingLogEntryStatus.TrackingLogId);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId, 
                                                                                                                            CancellationToken cancellationToken)
    {
        TrackingLogEntryStatus? status = await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                        .FilterById(trackingLogEntryStatusId)
                                                        .FirstOrDefaultAsync(cancellationToken);

        if (status is null)
        {
            return new List<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus>();
        }

        dbContext.Remove(status);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                              .Where(s => s.TrackingLogId == status.TrackingLogId)
                              .Select(s => s.ToDomain())
                              .ToListAsync(cancellationToken);
    }

    #endregion
}