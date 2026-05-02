#region Usings

using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Domain.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Workflow;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Features.Tracking;

public class TrackingRepository(TaskManagerDbContext dbContext) : ITrackingRepository
{
    #region Tracking Log

    public async Task<Domain.Tracking.TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog newTrackingLog, 
                                                                                  CancellationToken cancellationToken)
    {
        TrackingLog trackingLog = new()
                                  {
                                      Title = newTrackingLog.Title.Value,
                                      Description = newTrackingLog.Description?.Value,
                                      CreatedBy = newTrackingLog.CreatedById,
                                      CreatedAt = newTrackingLog.CreatedAt,
                                      UpdatedBy = newTrackingLog.CreatedById,
                                      UpdatedAt = newTrackingLog.CreatedAt
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
    
    public async Task<TrackingLogEntity?> GetTrackingLogEntityById(int trackingLogId,
                                                                   CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogs.FilterById(trackingLogId)
                              .Select(log => log.ToDomainEntity())
                              .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task Save(TrackingLogEntity log, 
                           CancellationToken cancellationToken)
    {
        TrackingLog? dbLog = await dbContext.TrackingLogs.FilterById(log.Id)
                                            .FirstOrDefaultAsync(cancellationToken);

        if (dbLog is null)
        {
            throw new InvariantException(actionResultType: ActionResultType.DataConflict, 
                                         message: MessageResources.ResourceDoesNotExist);
        }

        dbLog.Title = log.Title.Value;
        dbLog.Description = log.Description?.Value;
        dbLog.UpdatedBy = log.UpdatedBy;
        dbLog.UpdatedAt = log.UpdatedAt;
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Domain.Tracking.TrackingLog.TrackingLog>> DeleteTrackingLogById(int userId,
                                                                                           int trackingLogId,
                                                                                           CancellationToken cancellationToken)
    {
        await dbContext.TrackingLogs.FilterById(trackingLogId).ExecuteDeleteAsync(cancellationToken);

        return await GetAllTrackingLogs(userId, cancellationToken);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<Domain.Tracking.TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry logEntryToInsert, 
                                                                                                 CancellationToken cancellationToken)
    {
        TrackingLogEntry entry = new()
                                 {
                                     Title = logEntryToInsert.Title.Value,
                                     Description = logEntryToInsert.Description?.Value,
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

        entry.Title = updatableTrackingLogEntry.Title.Value;
        entry.Description = updatableTrackingLogEntry.Description?.Value;
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
        await dbContext.TrackingLogEntries.FilterById(trackingLogEntryId).ExecuteDeleteAsync(cancellationToken);

        return await GetAllTrackingLogEntries(userId, cancellationToken);
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert,
                                                                                                                   CancellationToken cancellationToken)
    {
        TrackingLogEntryStatus trackingLogEntryStatus = new()
                                                        {
                                                            Title = statusToInsert.Title.Value,
                                                            Description = statusToInsert.Description?.Value,
                                                            TrackingLogId = statusToInsert.TrackingLogId,
                                                            CreatedBy = statusToInsert.CreatedById,
                                                            CreatedAt = statusToInsert.CreatedAt,
                                                            UpdatedBy = statusToInsert.CreatedById,
                                                            UpdatedAt = statusToInsert.CreatedAt
                                                        };
        dbContext.TrackingLogEntryStatuses.Add(trackingLogEntryStatus);
        await dbContext.SaveChangesAsync(cancellationToken);

        return trackingLogEntryStatus.ToDomain();
    }

    public async Task<Domain.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus?> GetTrackingLogEntryStatusById(int id,
                                                                                                                    CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                              .Where(o => o.Id == id)
                              .Select(o => o.ToDomain())
                              .SingleOrDefaultAsync(cancellationToken);
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