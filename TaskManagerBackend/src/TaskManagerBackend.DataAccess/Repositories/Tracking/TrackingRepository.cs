#region Usings

using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public class TrackingRepository : ITrackingRepository
{
    private readonly TaskManagerDbContext _dbContext;

    public TrackingRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Tracking Log

    public async Task<TrackingLogGetResponse?> InsertTrackingLog(NewTrackingLog logToInsert)
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
        _dbContext.TrackingLogs.Add(trackingLog);
        await _dbContext.SaveChangesAsync();

        return await GetTrackingLogById(trackingLog.Id);
    }

    public async Task<List<TrackingLogGetResponse>> GetAllTrackingLogs(int userId)
    {
        return await _dbContext.TrackingLogs.AsNoTracking()
                                            .FilterByCreator(userId)
                                            .SelectEagerly()
                                            .Select(log => log.ToTrackingLogGetResponse())
                                            .ToListAsync();
    }

    public async Task<TrackingLogGetResponse?> GetTrackingLogById(int trackingLogId)
    {
        return await _dbContext.TrackingLogs.AsNoTracking()
                                            .FilterById(trackingLogId)
                                            .SelectEagerly()
                                            .Select(log => log.ToTrackingLogGetResponse())
                                            .FirstOrDefaultAsync();
    }

    public async Task<List<TrackingLogGetResponse>> DeleteTrackingLogById(int userId, int trackingLogId)
    {
        int deletedCount = await _dbContext.TrackingLogs.FilterById(trackingLogId).ExecuteDeleteAsync();

        if (deletedCount == 0)
        {
            return new List<TrackingLogGetResponse>();
        }

        return await GetAllTrackingLogs(userId);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<TrackingLogEntryGetResponse?> InsertTrackingLogEntry(NewTrackingLogEntry logEntryToInsert)
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
        _dbContext.TrackingLogEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        return await GetTrackingLogEntryById(entry.Id);
    }

    public async Task<List<TrackingLogEntryGetResponse>> GetAllTrackingLogEntries(int userId)
    {
        return await _dbContext.TrackingLogEntries.AsNoTracking()
                                                  .FilterByCreator(userId)
                                                  .Include(entry => entry.CreatedByNavigation)
                                                  .Include(entry => entry.UpdatedByNavigation)
                                                  .Include(entry => entry.TrackingLogEntryStatus)
                                                  .Select(entry => entry.ToTrackingLogEntryGetResponse())
                                                  .ToListAsync();
    }

    public async Task<TrackingLogEntryGetResponse?> GetTrackingLogEntryById(int trackingLogEntryId)
    {
        return await _dbContext.TrackingLogEntries.AsNoTracking()
                                                  .FilterById(trackingLogEntryId)
                                                  .Include(entry => entry.CreatedByNavigation)
                                                  .Include(entry => entry.UpdatedByNavigation)
                                                  .Include(entry => entry.TrackingLogEntryStatus)
                                                  .Select(entry => entry.ToTrackingLogEntryGetResponse())
                                                  .FirstOrDefaultAsync();
    }

    public async Task<TrackingLogEntryGetResponse?> UpdateTrackingLogEntryById(int id, 
                                                                               UpdatableTrackingLogEntry updatableTrackingLogEntry)
    {
        TrackingLogEntry? entry = await _dbContext.TrackingLogEntries.FilterById(id)
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
        await _dbContext.SaveChangesAsync();

        return await GetTrackingLogEntryById(id);
    }

    public async Task<List<TrackingLogEntryGetResponse>> DeleteTrackingLogEntryById(int userId,
                                                                                    int trackingLogEntryId)
    {
        int deletedCount = await _dbContext.TrackingLogEntries.FilterById(trackingLogEntryId)
                                                              .ExecuteDeleteAsync();

        if (deletedCount == 0)
        {
            return new List<TrackingLogEntryGetResponse>();
        }

        return await GetAllTrackingLogEntries(userId);
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<TrackingLogEntryStatusGetResponse?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert)
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
        _dbContext.TrackingLogEntryStatuses.Add(trackingLogEntryStatus);
        await _dbContext.SaveChangesAsync();

        return new TrackingLogEntryStatusGetResponse
               {
                   Id = trackingLogEntryStatus.Id,
                   Title = trackingLogEntryStatus.Title,
                   Description = trackingLogEntryStatus.Description,
                   TrackingLogId = trackingLogEntryStatus.TrackingLogId
               };
    }

    public async Task<List<TrackingLogEntryStatusGetResponse>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId)
    {
        TrackingLogEntryStatus? status = await _dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                                                  .FilterById(trackingLogEntryStatusId)
                                                                                  .FirstOrDefaultAsync();

        if (status is null)
        {
            return new List<TrackingLogEntryStatusGetResponse>();
        }

        _dbContext.Remove(status);
        await _dbContext.SaveChangesAsync();

        return await _dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                        .Where(s => s.TrackingLogId == status.TrackingLogId)
                                                        .Select(s => s.ToTrackingLogEntryStatus())
                                                        .ToListAsync();
    }

    #endregion
}