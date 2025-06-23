#region Usings

using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public class TrackingRepository : ITrackingRepository
{
    private readonly TaskManagerDbContext _dbContext;
    private readonly IDbConnectionProvider<SqlConnection> _dbConnectionProvider;

    public TrackingRepository(TaskManagerDbContext dbContext,
                              IDbConnectionProvider<SqlConnection> dbConnectionProvider)
    {
        _dbContext = dbContext;
        _dbConnectionProvider = dbConnectionProvider;
    }

    #region Tracking Log

    public async Task<TrackingLogGetResponse?> InsertTrackingLog(NewTrackingLog logToInsert)
    {
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

            return await GetTrackingLogById(trackingLog.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public async Task<List<TrackingLogGetResponse>> GetAllTrackingLogs(int userId)
    {
        return await _dbContext.TrackingLogs.Where(log => log.CreatedBy == userId)
                            .Select(log => new TrackingLogGetResponse
                                           {
                                               Id = log.Id,
                                               Title = log.Title,
                                               Description = log.Description,
                                               CreatedAt = log.CreatedAt,
                                               CreatedBy = new UserInfoDto()
                                                           {
                                                               Id = log.CreatedByNavigation.Id,
                                                               UserName = log.CreatedByNavigation.UserName,
                                                               Email = log.CreatedByNavigation.Email
                                                           },
                                               UpdatedAt = log.UpdatedAt,
                                               UpdatedBy = new UserInfoDto()
                                                           {
                                                               Id = log.UpdatedByNavigation.Id,
                                                               UserName = log.UpdatedByNavigation.UserName,
                                                               Email = log.UpdatedByNavigation.Email
                                                           },
                                               TrackingLogEntries = log
                                                                    .TrackingLogEntries
                                                                    .Select(entry => new TrackingLogEntryGetResponse
                                                                                     {
                                                                                         Id = entry.Id,
                                                                                         Title = entry.Title,
                                                                                         Description = entry.Description,
                                                                                         TrackingLogId = entry.TrackingLogId,
                                                                                         Status = new TrackingLogEntryStatus
                                                                                                  {
                                                                                                      Id = entry.Status.Id,
                                                                                                      Title = entry.Status.Title,
                                                                                                      Description = entry.Status
                                                                                                                         .Description,
                                                                                                      TrackingLogId = entry.TrackingLogId
                                                                                                  },
                                                                                         Priority = entry.Priority,
                                                                                         OrderIndex = (double)entry.OrderIndex,
                                                                                         CreatedAt = entry.CreatedAt,
                                                                                         CreatedBy = new UserInfoDto
                                                                                                     {
                                                                                                         Id = entry
                                                                                                              .CreatedByNavigation
                                                                                                              .Id,
                                                                                                         UserName = entry
                                                                                                                    .CreatedByNavigation
                                                                                                                    .UserName,
                                                                                                         Email = entry
                                                                                                                 .CreatedByNavigation
                                                                                                                 .Email
                                                                                                     },
                                                                                         UpdatedAt = entry.UpdatedAt,
                                                                                         UpdatedBy = new UserInfoDto
                                                                                                     {
                                                                                                         Id = entry
                                                                                                              .UpdatedByNavigation
                                                                                                              .Id,
                                                                                                         UserName = entry
                                                                                                                    .UpdatedByNavigation
                                                                                                                    .UserName,
                                                                                                         Email = entry
                                                                                                                 .UpdatedByNavigation
                                                                                                                 .Email
                                                                                                     },

                                                                                     })
                                                                    .ToList(),
                                               TrackingLogEntriesStatuses = log.Statuses.Select(s => new TrackingLogEntryStatus()
                                                                                                     {
                                                                                                         Id = s.Id,
                                                                                                         Title = s.Title,
                                                                                                         Description = s.Description,
                                                                                                         TrackingLogId =
                                                                                                             s.TrackingLogId
                                                                                                     })
                                                                               .ToList()
                                           })
                            .ToListAsync();
    }

    public async Task<TrackingLogGetResponse?> GetTrackingLogById(int trackingLogId)
    {
        try
        {
            return await _dbContext.TrackingLogs.Where(log => log.Id == trackingLogId)
                            .Select(log => new TrackingLogGetResponse
                                           {
                                               Id = log.Id,
                                               Title = log.Title,
                                               Description = log.Description,
                                               CreatedAt = log.CreatedAt,
                                               CreatedBy = new UserInfoDto()
                                                           {
                                                               Id = log.CreatedByNavigation.Id,
                                                               UserName = log.CreatedByNavigation.UserName,
                                                               Email = log.CreatedByNavigation.Email
                                                           },
                                               UpdatedAt = log.UpdatedAt,
                                               UpdatedBy = new UserInfoDto()
                                                           {
                                                               Id = log.UpdatedByNavigation.Id,
                                                               UserName = log.UpdatedByNavigation.UserName,
                                                               Email = log.UpdatedByNavigation.Email
                                                           },
                                               TrackingLogEntries = log
                                                                    .TrackingLogEntries
                                                                    .Select(entry => new TrackingLogEntryGetResponse
                                                                                     {
                                                                                         Id = entry.Id,
                                                                                         Title = entry.Title,
                                                                                         Description = entry.Description,
                                                                                         TrackingLogId = entry.TrackingLogId,
                                                                                         Status = new TrackingLogEntryStatus
                                                                                                  {
                                                                                                      Id = entry.Status.Id,
                                                                                                      Title = entry.Status.Title,
                                                                                                      Description = entry.Status
                                                                                                                         .Description,
                                                                                                      TrackingLogId = trackingLogId
                                                                                                  },
                                                                                         Priority = entry.Priority,
                                                                                         OrderIndex = (double)entry.OrderIndex,
                                                                                         CreatedAt = entry.CreatedAt,
                                                                                         CreatedBy = new UserInfoDto
                                                                                                     {
                                                                                                         Id = entry
                                                                                                              .CreatedByNavigation
                                                                                                              .Id,
                                                                                                         UserName = entry
                                                                                                                    .CreatedByNavigation
                                                                                                                    .UserName,
                                                                                                         Email = entry
                                                                                                                 .CreatedByNavigation
                                                                                                                 .Email
                                                                                                     },
                                                                                         UpdatedAt = entry.UpdatedAt,
                                                                                         UpdatedBy = new UserInfoDto
                                                                                                     {
                                                                                                         Id = entry
                                                                                                              .UpdatedByNavigation
                                                                                                              .Id,
                                                                                                         UserName = entry
                                                                                                                    .UpdatedByNavigation
                                                                                                                    .UserName,
                                                                                                         Email = entry
                                                                                                                 .UpdatedByNavigation
                                                                                                                 .Email
                                                                                                     },

                                                                                     })
                                                                    .ToList(),
                                               TrackingLogEntriesStatuses = log.Statuses.Select(s => new TrackingLogEntryStatus()
                                                                                                     {
                                                                                                         Id = s.Id,
                                                                                                         Title = s.Title,
                                                                                                         Description = s.Description,
                                                                                                         TrackingLogId =
                                                                                                             trackingLogId
                                                                                                     })
                                                                               .ToList()
                                           })
                            .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<TrackingLogGetResponse>> DeleteTrackingLogById(int userId, int trackingLogId)
    {
        await _dbContext.TrackingLogs.Where(log => log.Id == trackingLogId).ExecuteDeleteAsync();

        return await GetAllTrackingLogs(userId);
    }
    
    #endregion

    #region Tracking Log Entries

    public async Task<TrackingLogEntryGetResponse?> InsertTrackingLogEntry(NewTrackingLogEntry logEntryToInsert)
    {
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        int id = await connection.ExecuteScalarAsync<int>(
                                                          @"insert into [TrackingLogEntry] (Title, Description, TrackingLogId, StatusId, Priority, OrderIndex, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) values 
(@Title, @Description, @TrackingLogId, @StatusId, @Priority, @OrderIndex, @CreatedById, @CreatedAt, @CreatedById, @CreatedAt); 

select scope_identity();",
                                                          logEntryToInsert);

        return await GetTrackingLogEntryByIdInternal(connection, id);
    }

    public async Task<List<TrackingLogEntryGetResponse>> GetAllTrackingLogEntries(int userId)
    {
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        return await GetAllTrackingLogEntriesInternal(connection, userId);
    }

    public async Task<TrackingLogEntryGetResponse?> GetTrackingLogEntryById(int trackingLogEntryId)
    {
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        return await GetTrackingLogEntryByIdInternal(connection, trackingLogEntryId);
    }

    public async Task<TrackingLogEntryGetResponse?> UpdateTrackingLogEntryById(int id, 
                                                                               UpdatableTrackingLogEntry updatableTrackingLogEntry)
    {
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        string sql = @"UPDATE [TrackingLogEntry]
SET
    [TrackingLogEntry].[Title] = @Title,
    [TrackingLogEntry].[Description] = @Description,
    [TrackingLogEntry].[TrackingLogId] = @TrackingLogId,
    [TrackingLogEntry].[StatusId] = @StatusId,
    [TrackingLogEntry].[Priority] = @Priority,
    [TrackingLogEntry].[OrderIndex] = @OrderIndex,
    [TrackingLogEntry].[UpdatedBy] = @UpdatedBy,
    [TrackingLogEntry].[UpdatedAt] = @UpdatedAt
WHERE
    [TrackingLogEntry].[Id] = @Id";

        int result = await connection.ExecuteAsync(sql, 
                                                   param: new 
                                                          {
                                                              Id = id, 
                                                              updatableTrackingLogEntry.Title, 
                                                              updatableTrackingLogEntry.Description,
                                                              updatableTrackingLogEntry.TrackingLogId,
                                                              updatableTrackingLogEntry.StatusId,
                                                              updatableTrackingLogEntry.Priority,
                                                              updatableTrackingLogEntry.OrderIndex,
                                                              updatableTrackingLogEntry.UpdatedBy,
                                                              updatableTrackingLogEntry.UpdatedAt
                                                          });

        if (result == 0)
        {
            return null;
        }

        if (result == 1)
        {
            return await GetTrackingLogEntryByIdInternal(connection, id); 
        }

        throw new UnreachableException("Inconsistent data. For unique entity multiple rows were changed.");
    }

    public async Task<List<TrackingLogEntryGetResponse>> DeleteTrackingLogEntryById(int userId,
                                                                                    int trackingLogEntryId)
    {
        await using SqlConnection connection = _dbConnectionProvider.GetConnection();

        string sql = "delete from [TrackingLogEntry] where [TrackingLogEntry].[Id]=@TrackingLogEntryId";
        await connection.ExecuteAsync(sql,
                                      new { TrackingLogEntryId = trackingLogEntryId });

        return await GetAllTrackingLogEntriesInternal(connection, userId);
    }

    #region Internal

    private async Task<TrackingLogEntryGetResponse?> GetTrackingLogEntryByIdInternal(SqlConnection connection,
                                                                                     int id)
    {
        string sql =
            @"SELECT [TLE].[Id], [TLE].[Title], [TLE].[Description], [TLE].[TrackingLogId], [TLE].[StatusId],
[TLE].[Priority], [TLE].[OrderIndex], [TLE].[CreatedBy], [TLE].[CreatedAt], [TLE].[UpdatedBy], [TLE].[UpdatedAt],
[S].[Id], [S].[TrackingLogId], [S].[Title], [S].[Description],
[Creator].[Id], [Creator].[UserName], [Creator].[Email],
[Updater].[Id], [Updater].[UserName], [Updater].[Email]
FROM [TrackingLogEntry] as [TLE]
INNER JOIN [Status] as [S]
ON [TLE].[StatusId] = [S].[Id]
INNER JOIN [User] as [Creator]
ON [TLE].[CreatedBy] = [Creator].[Id]
INNER JOIN [User] as [Updater]
ON [TLE].[UpdatedBy] = [Updater].[Id]
WHERE [TLE].[Id] = @Id";

        IEnumerable<TrackingLogEntryGetResponse> logEntryData =
            await connection.QueryAsync<TrackingLogEntryData, 
                                        TrackingLogEntryStatus, 
                                        UserInfoDto, 
                                        UserInfoDto, 
                                        TrackingLogEntryGetResponse>(sql,
                                                                     (logEntry, status, creator, updater) => 
                                                                         new TrackingLogEntryGetResponse()
                                                                         {
                                                                             Id = logEntry.Id,
                                                                             Title = logEntry.Title,
                                                                             Description = logEntry.Description,
                                                                             TrackingLogId = logEntry.TrackingLogId,
                                                                             Status = status,
                                                                             Priority = logEntry.Priority,
                                                                             OrderIndex = logEntry.OrderIndex,
                                                                             CreatedBy = creator,
                                                                             CreatedAt = logEntry.CreatedAt,
                                                                             UpdatedBy = updater,
                                                                             UpdatedAt = logEntry.UpdatedAt,
                                                                         },
                                                                     splitOn: "Id, Id, Id",
                                                                     param: new { Id = id });

        return logEntryData.FirstOrDefault();
    }

    private async Task<List<TrackingLogEntryGetResponse>> GetAllTrackingLogEntriesInternal(SqlConnection connection,
                                                                                           int userId)
    {
        string sql =
            @"SELECT [TLE].[Id], [TLE].[Title], [TLE].[Description], [TLE].[TrackingLogId], [TLE].[StatusId],
[TLE].[Priority], [TLE].[OrderIndex], [TLE].[CreatedBy], [TLE].[CreatedAt], [TLE].[UpdatedBy], [TLE].[UpdatedAt],
[S].[Id], [S].[TrackingLogId], [S].[Title], [S].[Description],
[Creator].[Id], [Creator].[UserName], [Creator].[Email],
[Updater].[Id], [Updater].[UserName], [Updater].[Email]
FROM [TrackingLogEntry] as [TLE]
INNER JOIN [Status] as [S]
ON [TLE].[StatusId] = [S].[Id]
INNER JOIN [User] as [Creator]
ON [TLE].[CreatedBy] = [Creator].[Id]
INNER JOIN [User] as [Updater]
ON [TLE].[UpdatedBy] = [Updater].[Id]
where [TLE].[CreatedBy] = @UserId";

        IEnumerable<TrackingLogEntryGetResponse> logEntryData =
            await connection.QueryAsync<TrackingLogEntryData,
                TrackingLogEntryStatus,
                UserInfoDto,
                UserInfoDto,
                TrackingLogEntryGetResponse>(
                                             sql,
                                             (logEntry,
                                              status,
                                              creator,
                                              updater) => new TrackingLogEntryGetResponse()
                                                          {
                                                              Id = logEntry.Id,
                                                              Title = logEntry.Title,
                                                              Description = logEntry.Description,
                                                              TrackingLogId = logEntry.TrackingLogId,
                                                              Status = status,
                                                              Priority = logEntry.Priority,
                                                              OrderIndex = logEntry.OrderIndex,
                                                              CreatedBy = creator,
                                                              CreatedAt = logEntry.CreatedAt,
                                                              UpdatedBy = updater,
                                                              UpdatedAt = logEntry.UpdatedAt,
                                                          },
                                             splitOn: "Id, Id, Id",
                                             param: new
                                                    {
                                                        UserId = userId
                                                    });

        return logEntryData.Distinct().ToList();
    }

    #endregion

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert)
    {
        try
        {
            Status status = new()
                            {
                                Title = statusToInsert.Title,
                                Description = statusToInsert.Description,
                                TrackingLogId = statusToInsert.TrackingLogId,
                                CreatedBy = statusToInsert.CreatedById,
                                CreatedAt = statusToInsert.CreatedAt,
                                UpdatedBy = statusToInsert.CreatedById,
                                UpdatedAt = statusToInsert.CreatedAt
                            };
            _dbContext.Statuses.Add(status);
            await _dbContext.SaveChangesAsync();

            return new TrackingLogEntryStatus
                   {
                       Id = status.Id,
                       Title = status.Title,
                       Description = status.Description,
                       TrackingLogId = status.TrackingLogId
                   };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public async Task<List<TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId)
    {
        Status? status = await _dbContext.Statuses
                                         .AsNoTracking()
                                         .Where(s => s.Id == trackingLogEntryStatusId)
                                         .FirstOrDefaultAsync();

        if (status is null)
        {
            return new List<TrackingLogEntryStatus>();
        }

        _dbContext.Remove(status);
        await _dbContext.SaveChangesAsync();

        return await _dbContext.Statuses.AsNoTracking()
                                        .Where(s => s.TrackingLogId == status.TrackingLogId)
                                        .Select(s => new TrackingLogEntryStatus
                                                     {
                                                         Id = s.Id,
                                                         Title = s.Title,
                                                         Description = s.Description,
                                                         TrackingLogId = s.TrackingLogId
                                                     })
                                        .ToListAsync();
    }

    #endregion
}