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

    public async Task<Domain.Tracking.TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog logToInsert)
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
        await dbContext.SaveChangesAsync();

        return await GetTrackingLogById(trackingLog.Id);
    }

    public async Task<List<Domain.Tracking.TrackingLog.TrackingLog>> GetAllTrackingLogs(int userId)
    {
        return await dbContext.TrackingLogs.AsNoTracking()
                              .FilterByCreator(userId)
                              .SelectEagerly()
                              .Select(log => log.ToDomain())
                              .ToListAsync();
    }

    public async Task<Domain.Tracking.TrackingLog.TrackingLog?> GetTrackingLogById(int trackingLogId)
    {
        return await dbContext.TrackingLogs.AsNoTracking()
                              .FilterById(trackingLogId)
                              .SelectEagerly()
                              .Select(log => log.ToDomain())
                              .FirstOrDefaultAsync();
    }

    public async Task<List<Domain.Tracking.TrackingLog.TrackingLog>> DeleteTrackingLogById(int userId, int trackingLogId)
    {
        int deletedCount = await dbContext.TrackingLogs.FilterById(trackingLogId).ExecuteDeleteAsync();

        if (deletedCount == 0)
        {
            return new List<Domain.Tracking.TrackingLog.TrackingLog>();
        }

        return await GetAllTrackingLogs(userId);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry logEntryToInsert)
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
        await dbContext.SaveChangesAsync();

        return await GetTrackingLogEntryById(entry.Id);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntry.TrackingLogEntry>> GetAllTrackingLogEntries(int userId)
    {
        return await dbContext.TrackingLogEntries.AsNoTracking()
                              .FilterByCreator(userId)
                              .Include(entry => entry.CreatedByNavigation)
                              .Include(entry => entry.UpdatedByNavigation)
                              .Include(entry => entry.TrackingLogEntryStatus)
                              .Select(entry => entry.ToDomain())
                              .ToListAsync();
    }

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId)
    {
        return await dbContext.TrackingLogEntries.AsNoTracking()
                              .FilterById(trackingLogEntryId)
                              .Include(entry => entry.CreatedByNavigation)
                              .Include(entry => entry.UpdatedByNavigation)
                              .Include(entry => entry.TrackingLogEntryStatus)
                              .Select(entry => entry.ToDomain())
                              .FirstOrDefaultAsync();
    }

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> UpdateTrackingLogEntryById(int trackingLogEntryId,
                                                                                    UpdatableTrackingLogEntry updatableTrackingLogEntry)
    {
        TrackingLogEntry? entry = await dbContext.TrackingLogEntries.FilterById(trackingLogEntryId)
                                                 .FirstOrDefaultAsync();

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
        await dbContext.SaveChangesAsync();

        return await GetTrackingLogEntryById(trackingLogEntryId);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntry.TrackingLogEntry>> DeleteTrackingLogEntryById(int userId, 
                                                                                         int trackingLogEntryId)
    {
        int deletedCount = await dbContext.TrackingLogEntries.FilterById(trackingLogEntryId)
                                          .ExecuteDeleteAsync();

        if (deletedCount == 0)
        {
            return new List<Domain.Tracking.TrackingLogEntry.TrackingLogEntry>();
        }

        return await GetAllTrackingLogEntries(userId);
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert)
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
        await dbContext.SaveChangesAsync();

        return new Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus(trackingLogEntryStatus.Id,
                                                          trackingLogEntryStatus.Title,
                                                          trackingLogEntryStatus.Description,
                                                          trackingLogEntryStatus.TrackingLogId);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId)
    {
        TrackingLogEntryStatus? status = await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                        .FilterById(trackingLogEntryStatusId)
                                                        .FirstOrDefaultAsync();

        if (status is null)
        {
            return new List<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus>();
        }

        dbContext.Remove(status);
        await dbContext.SaveChangesAsync();

        return await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                              .Where(s => s.TrackingLogId == status.TrackingLogId)
                              .Select(s => s.ToDomain())
                              .ToListAsync();
    }

    #endregion
}