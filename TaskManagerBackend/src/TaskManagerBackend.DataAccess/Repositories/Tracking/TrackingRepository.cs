#region Usings

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.Tracking;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntryStatus;

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

    public async Task<Domain.Tracking.TrackingLog?> InsertTrackingLog(NewTrackingLog logToInsert)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
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
            
            TrackingLogCreated domainEvent = new(Guid.NewGuid(),
                                                 trackingLog.Id,
                                                 logToInsert,
                                                 Guid.NewGuid());

            Event dbEvent = new()
                            {
                                Id = domainEvent.Id,
                                EntityType = domainEvent.EntityType,
                                EntityId = domainEvent.EntityId,
                                EntityVersion = domainEvent.EntityVersion,
                                Data = JsonSerializer.Serialize(domainEvent.Data),
                                DispatchedByUserId = domainEvent.DispatchedByUserId,
                                DispatchedAt = domainEvent.DispatchedAt,
                                CorrelationId = domainEvent.CorrelationId
                            };
        
            _dbContext.Events.Add(dbEvent);
            
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetTrackingLogById(trackingLog.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Domain.Tracking.TrackingLog>> GetAllTrackingLogs(int userId)
    {
        return await _dbContext.TrackingLogs.AsNoTracking()
                                            .FilterByCreator(userId)
                                            .SelectEagerly()
                                            .Select(log => log.ToDomain())
                                            .ToListAsync();
    }

    public async Task<Domain.Tracking.TrackingLog?> GetTrackingLogById(int trackingLogId)
    {
        return await _dbContext.TrackingLogs.AsNoTracking()
                                            .FilterById(trackingLogId)
                                            .SelectEagerly()
                                            .Select(log => log.ToDomain())
                                            .FirstOrDefaultAsync();
    }

    public async Task<List<Domain.Tracking.TrackingLog>> DeleteTrackingLogById(int userId, int trackingLogId)
    {
        int deletedCount = await _dbContext.TrackingLogs.FilterById(trackingLogId).ExecuteDeleteAsync();

        if (deletedCount == 0)
        {
            return new List<Domain.Tracking.TrackingLog>();
        }

        return await GetAllTrackingLogs(userId);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<Domain.Tracking.TrackingLogEntry?> InsertTrackingLogEntry(NewTrackingLogEntry logEntryToInsert)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            TrackingLogEntry entry = new()
                                     {
                                         Title = logEntryToInsert.Title,
                                         Description = logEntryToInsert.Description,
                                         TrackingLogId = logEntryToInsert.TrackingLogId,
                                         StatusId = logEntryToInsert.StatusId,
                                         Priority = logEntryToInsert.Priority,
                                         OrderIndex = logEntryToInsert.OrderIndex,
                                         CreatedBy = logEntryToInsert.CreatedById,
                                         CreatedAt = logEntryToInsert.CreatedAt,
                                         UpdatedBy = logEntryToInsert.CreatedById,
                                         UpdatedAt = logEntryToInsert.CreatedAt
                                     };
            
            _dbContext.TrackingLogEntries.Add(entry);
            
            await _dbContext.SaveChangesAsync();
            
            TrackingLogEntryCreated domainEvent = new(Guid.NewGuid(), 
                                                      entry.Id, 
                                                      logEntryToInsert,
                                                      Guid.NewGuid());

            Event dbEvent = new()
                            {
                                Id = domainEvent.Id,
                                EntityType = domainEvent.EntityType,
                                EntityId = domainEvent.EntityId,
                                EntityVersion = domainEvent.EntityVersion,
                                Data = JsonSerializer.Serialize(domainEvent.Data),
                                DispatchedByUserId = domainEvent.DispatchedByUserId,
                                DispatchedAt = domainEvent.DispatchedAt,
                                CorrelationId = domainEvent.CorrelationId
                            };
        
            _dbContext.Events.Add(dbEvent);
            
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetTrackingLogEntryById(entry.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Domain.Tracking.TrackingLogEntry>> GetAllTrackingLogEntries(int userId)
    {
        return await _dbContext.TrackingLogEntries.AsNoTracking()
                                                  .FilterByCreator(userId)
                                                  .Include(entry => entry.CreatedByNavigation)
                                                  .Include(entry => entry.UpdatedByNavigation)
                                                  .Include(entry => entry.TrackingLogEntryStatus)
                                                  .Select(entry => entry.ToDomain())
                                                  .ToListAsync();
    }

    public async Task<Domain.Tracking.TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId)
    {
        return await _dbContext.TrackingLogEntries.AsNoTracking()
                                                  .FilterById(trackingLogEntryId)
                                                  .Include(entry => entry.CreatedByNavigation)
                                                  .Include(entry => entry.UpdatedByNavigation)
                                                  .Include(entry => entry.TrackingLogEntryStatus)
                                                  .Select(entry => entry.ToDomain())
                                                  .FirstOrDefaultAsync();
    }

    public async Task<Domain.Tracking.TrackingLogEntry?> UpdateTrackingLogEntryById(int id, 
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
        entry.OrderIndex = updatableTrackingLogEntry.OrderIndex;
        entry.UpdatedBy = updatableTrackingLogEntry.UpdatedBy;
        entry.UpdatedAt = updatableTrackingLogEntry.UpdatedAt;
        
        int lastEntityVersion = await _dbContext.Events.Where(e => e.EntityType == EntityType.TrackingLogEntry.Id &&
                                                                   e.EntityId == id)
                                                       .Select(e => e.EntityVersion)
                                                       .OrderByDescending(o => o)
                                                       .FirstAsync();
        
        TrackingLogEntryUpdated domainEvent = new(Guid.NewGuid(), 
                                                  entry.Id,
                                                  lastEntityVersion + 1,
                                                  updatableTrackingLogEntry,
                                                  Guid.NewGuid());

        Event dbEvent = new()
                        {
                            Id = domainEvent.Id,
                            EntityType = domainEvent.EntityType,
                            EntityId = domainEvent.EntityId,
                            EntityVersion = domainEvent.EntityVersion,
                            Data = JsonSerializer.Serialize(domainEvent.Data),
                            DispatchedByUserId = domainEvent.DispatchedByUserId,
                            DispatchedAt = domainEvent.DispatchedAt,
                            CorrelationId = domainEvent.CorrelationId
                        };
        
        _dbContext.Events.Add(dbEvent);
        
        await _dbContext.SaveChangesAsync();

        return await GetTrackingLogEntryById(id);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntry>> DeleteTrackingLogEntryById(int userId, 
                                                                                         int trackingLogEntryId)
    {
        int deletedCount = await _dbContext.TrackingLogEntries.FilterById(trackingLogEntryId)
                                                              .ExecuteDeleteAsync();

        if (deletedCount == 0)
        {
            return new List<Domain.Tracking.TrackingLogEntry>();
        }

        return await GetAllTrackingLogEntries(userId);
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<Domain.Tracking.TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert)
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

        return new Domain.Tracking.TrackingLogEntryStatus(trackingLogEntryStatus.Id,
                                                          trackingLogEntryStatus.Title,
                                                          trackingLogEntryStatus.Description,
                                                          trackingLogEntryStatus.TrackingLogId);
    }

    public async Task<List<Domain.Tracking.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId)
    {
        TrackingLogEntryStatus? status = await _dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                                                  .FilterById(trackingLogEntryStatusId)
                                                                                  .FirstOrDefaultAsync();

        if (status is null)
        {
            return new List<Domain.Tracking.TrackingLogEntryStatus>();
        }

        _dbContext.Remove(status);
        await _dbContext.SaveChangesAsync();

        return await _dbContext.TrackingLogEntryStatuses.AsNoTracking()
                                                        .Where(s => s.TrackingLogId == status.TrackingLogId)
                                                        .Select(s => s.ToDomain())
                                                        .ToListAsync();
    }

    #endregion
}