#region Usings

using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.Domain.Features.Tracking;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Shared.Data;
using TaskManagerBackend.Domain.Shared.Workflow;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Features.Tracking;

public class TrackingRepository(TaskManagerDbContext dbContext) : ITrackingRepository
{
    #region Tracking Log

    public async Task<Domain.Features.Tracking.TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog newTrackingLog, 
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

    public async Task<List<Domain.Features.Tracking.TrackingLog.TrackingLog>> GetAllTrackingLogs(int userId, 
                                                                                                 CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogs.AsNoTracking()
                              .FilterByCreator(userId)
                              .SelectEagerly()
                              .Select(log => log.ToDomain())
                              .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Features.Tracking.TrackingLog.TrackingLog?> GetTrackingLogById(int id, 
                                                                                            CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogs.AsNoTracking()
                              .FilterById(id)
                              .SelectEagerly()
                              .Select(log => log.ToDomain())
                              .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<TrackingLogEntity?> GetTrackingLogEntityById(int id,
                                                                   CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogs.FilterById(id)
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

    public async Task<List<Domain.Features.Tracking.TrackingLog.TrackingLog>> DeleteTrackingLogById(int id,
                                                                                                    int userId,
                                                                                                    CancellationToken cancellationToken)
    {
        await dbContext.TrackingLogs.FilterById(id).ExecuteDeleteAsync(cancellationToken);

        return await GetAllTrackingLogs(userId, cancellationToken);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry newTrackingLogEntry, 
                                                                                                          CancellationToken cancellationToken)
    {
        TrackingLogEntry entry = new()
                                 {
                                     Title = newTrackingLogEntry.Title.Value,
                                     Description = newTrackingLogEntry.Description?.Value,
                                     TrackingLogId = newTrackingLogEntry.TrackingLogId,
                                     StatusId = newTrackingLogEntry.StatusId,
                                     Priority = newTrackingLogEntry.Priority,
                                     OrderIndex = (decimal) newTrackingLogEntry.OrderIndex,
                                     CreatedBy = newTrackingLogEntry.CreatedById,
                                     CreatedAt = newTrackingLogEntry.CreatedAt,
                                     UpdatedBy = newTrackingLogEntry.CreatedById,
                                     UpdatedAt = newTrackingLogEntry.CreatedAt
                                 };
        dbContext.TrackingLogEntries.Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetTrackingLogEntryById(entry.Id, cancellationToken);
    }

    public async Task<List<Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry>> GetAllTrackingLogEntries(int userId,
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

    public async Task<Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry?> GetTrackingLogEntryById(int id,
                                                                                                           CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogEntries.AsNoTracking()
                              .FilterById(id)
                              .Include(entry => entry.CreatedByNavigation)
                              .Include(entry => entry.UpdatedByNavigation)
                              .Include(entry => entry.TrackingLogEntryStatus)
                              .Select(entry => entry.ToDomain())
                              .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TrackingLogEntryEntity?> GetTrackingLogEntryEntityById(int id,
                                                                             CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogEntries.FilterById(id)
                              .Select(log => log.ToDomainEntity())
                              .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry?> UpdateTrackingLogEntryById(int id, 
        UpdatableTrackingLogEntry updatableTrackingLogEntry,
        CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await dbContext.TrackingLogEntries.FilterById(id)
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
        entry.OrderIndex = updatableTrackingLogEntry.OrderIndex;
        entry.UpdatedBy = updatableTrackingLogEntry.UpdatedBy;
        entry.UpdatedAt = updatableTrackingLogEntry.UpdatedAt;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetTrackingLogEntryById(id, cancellationToken);
    }
    
    public async Task Save(TrackingLogEntryEntity logEntry, 
                           CancellationToken cancellationToken)
    {
        TrackingLogEntry? dbLogEntry = await dbContext.TrackingLogEntries.FilterById(logEntry.Id)
                                                      .FirstOrDefaultAsync(cancellationToken);

        if (dbLogEntry is null)
        {
            throw new InvariantException(actionResultType: ActionResultType.DataConflict, 
                                         message: MessageResources.ResourceDoesNotExist);
        }

        dbLogEntry.Title = logEntry.Title.Value;
        dbLogEntry.StatusId = logEntry.TrackingLogEntryStatusId;
        dbLogEntry.Description = logEntry.Description?.Value;
        dbLogEntry.UpdatedBy = logEntry.UpdatedBy;
        dbLogEntry.UpdatedAt = logEntry.UpdatedAt;
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry>> DeleteTrackingLogEntryById(int id,
                                                                                                                   int userId,
                                                                                                                   CancellationToken cancellationToken)
    {
        await dbContext.TrackingLogEntries.FilterById(id).ExecuteDeleteAsync(cancellationToken);

        return await GetAllTrackingLogEntries(userId, cancellationToken);
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<Domain.Features.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(
        NewTrackingLogEntryStatus newStatus,
        CancellationToken cancellationToken)
    {
        TrackingLogEntryStatus trackingLogEntryStatus = new()
                                                        {
                                                            Title = newStatus.Title.Value,
                                                            Description = newStatus.Description?.Value,
                                                            TrackingLogId = newStatus.TrackingLogId,
                                                            CreatedBy = newStatus.CreatedById,
                                                            CreatedAt = newStatus.CreatedAt,
                                                            UpdatedBy = newStatus.CreatedById,
                                                            UpdatedAt = newStatus.CreatedAt
                                                        };
        dbContext.TrackingLogEntryStatuses.Add(trackingLogEntryStatus);
        await dbContext.SaveChangesAsync(cancellationToken);

        return trackingLogEntryStatus.ToDomain();
    }

    public async Task<Domain.Features.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus?> GetTrackingLogEntryStatusById(int id, 
                                                                                                                             CancellationToken cancellationToken)
    {
        return await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                              .Where(o => o.Id == id)
                              .Select(o => o.ToDomain())
                              .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Domain.Features.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int id, 
                                                                                                                                     CancellationToken cancellationToken)
    {
        TrackingLogEntryStatus? status = await dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                        .FilterById(id)
                                                        .FirstOrDefaultAsync(cancellationToken);

        if (status is null)
        {
            return new List<Domain.Features.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus>();
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