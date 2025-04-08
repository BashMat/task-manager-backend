#region Usings

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagerBackend.Common;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public class TrackingRepository : ITrackingRepository
{
    private readonly IConfiguration _configuration;

    public TrackingRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    #region Tracking Log

    public async Task<TrackingLogGetResponse?> Insert(NewTrackingLog logToInsert)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        int id = await connection.ExecuteScalarAsync<int>(
                                                          @"insert into [TrackingLog] (Title, Description, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) values 
(@Title, @Description, @CreatedById, @CreatedAt, @CreatedById, @CreatedAt); 

select scope_identity();",
                                                          logToInsert);

        return await GetByIdInternal(connection, id);
    }

    public async Task<List<TrackingLogGetResponse>> GetAll(int userId)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        return await GetAllInternal(connection, userId);
    }

    public async Task<TrackingLogGetResponse?> GetById(int trackingLogId)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        return await GetByIdInternal(connection, trackingLogId);
    }

    public async Task<List<TrackingLogGetResponse>> Delete(int userId, int trackingLogId)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        await connection.ExecuteAsync("delete from [TrackingLog] where [TrackingLog].[Id]=@trackingLogId",
                                      new { TrackingLogId = trackingLogId });

        return await GetAllInternal(connection, userId);
    }

    #region Internal

    private async Task<TrackingLogGetResponse?> GetByIdInternal(SqlConnection connection, int trackingLogId)
    {
        TrackingLogGetResponse? trackingLog = await GetTrackingLogWithEntriesById(connection, trackingLogId);

        if (trackingLog is null)
        {
            return trackingLog;
        }
        
        List<TrackingLogEntryStatus> trackingLogStatuses = await GetTrackingLogStatusesById(connection, trackingLogId);
        
        trackingLog.TrackingLogEntriesStatuses = trackingLogStatuses;
        return trackingLog;
    }

    private async Task<List<TrackingLogEntryStatus>> GetTrackingLogStatusesById(SqlConnection connection, int trackingLogId)
    {
        IEnumerable<TrackingLogEntryStatus> trackingLogs = await connection.QueryAsync<TrackingLogEntryStatus>(
                                                            @"select [S].[Id], [S].[TrackingLogId], [S].[Title], [S].[Description]
from [TrackingLog] as [TL] 
inner join [Status] as [S] on [S].[TrackingLogId] = [TL].[Id] 
where [TL].[Id] = @trackingLogId",
                                                            param: new { TrackingLogId = trackingLogId });

        return trackingLogs.ToList();
    }

    private async Task<TrackingLogGetResponse?> GetTrackingLogWithEntriesById(SqlConnection connection, int trackingLogId)
    {
        Dictionary<int, TrackingLogGetResponse> historyTrackingLogs = new();
        Dictionary<int, TrackingLogEntryGetResponse> historyTrackingLogEntries = new();
        Type[] types = {
                           typeof(TrackingLogGetResponse),
                           typeof(UserInfoDto),
                           typeof(UserInfoDto),
                           typeof(TrackingLogEntryGetResponse),
                           typeof(UserInfoDto),
                           typeof(UserInfoDto),
                           typeof(TrackingLogEntryStatus)
                       };

        IEnumerable<TrackingLogGetResponse> trackingLogs = await connection.QueryAsync<TrackingLogGetResponse>(
                                                   @"select [TL].[Id], [TL].[Title], [TL].[Description], [TL].[CreatedAt], [TL].[UpdatedAt], 
[TrackingLogCreator].[Id], [TrackingLogCreator].[UserName], [TrackingLogCreator].[FirstName], [TrackingLogCreator].[LastName], [TrackingLogCreator].[Email], 
[TrackingLogUpdater].[Id], [TrackingLogUpdater].[UserName], [TrackingLogUpdater].[FirstName], [TrackingLogUpdater].[LastName], [TrackingLogUpdater].[Email], 
[TLE].[Id], [TLE].[TrackingLogId], [TLE].[StatusId], [TLE].[Title], [TLE].[Description], [TLE].[CreatedAt], [TLE].[UpdatedAt], 
[TLECreator].[Id], [TLECreator].[UserName], [TLECreator].[FirstName], [TLECreator].[LastName], [TLECreator].[Email], 
[TLEUpdater].[Id], [TLEUpdater].[UserName], [TLEUpdater].[FirstName], [TLEUpdater].[LastName], [TLEUpdater].[Email],
[S].[Id], [S].[Title], [S].[Description]
from [TrackingLog] as [TL] 
inner join [User] as [TrackingLogCreator] on [TL].[CreatedBy] = [TrackingLogCreator].[Id] 
inner join [User] as [TrackingLogUpdater] on [TL].[UpdatedBy] = [TrackingLogUpdater].[Id] 
left join [TrackingLogEntry] as [TLE] on [TL].[Id] = [TLE].[TrackingLogId] 
left join [User] as [TLECreator] on [TLE].[CreatedBy] = [TLECreator].[Id] 
left join [User] as [TLEUpdater] on [TLE].[UpdatedBy] = [TLEUpdater].[Id]
left join [Status] as [S] on [TLE].[StatusId] = [S].[Id] 
where [TL].[Id] = @trackingLogId",
                                                   types,
                                                   typesArg =>
                                                       ToDto(historyTrackingLogs,
                                                             historyTrackingLogEntries,
                                                             (TrackingLogGetResponse)typesArg[0],
                                                             (UserInfoDto)typesArg[1],
                                                             (UserInfoDto)typesArg[2],
                                                             (TrackingLogEntryGetResponse?)typesArg[3],
                                                             (UserInfoDto?)typesArg[4],
                                                             (UserInfoDto?)typesArg[5],
                                                             (TrackingLogEntryStatus?)typesArg[6]),
                                                   param: new { TrackingLogId = trackingLogId },
                                                   splitOn: "Id, Id, Id, Id, Id");

        List<TrackingLogGetResponse> listBoards = trackingLogs.Distinct().ToList();
        return listBoards.Count == 0
                   ? null 
                   : listBoards[0];
    }

    private async Task<List<TrackingLogGetResponse>> GetAllInternal(SqlConnection connection, int userId)
    {
        List<TrackingLogGetResponse> trackingLogs = await GetAllTrackingLogsWithEntriesByUserId(connection, userId);

        if (trackingLogs.Count == 0)
        {
            return trackingLogs;
        }
        
        List<TrackingLogEntryStatus> trackingLogStatuses = await GetAllTrackingLogsStatusesByUserId(connection, userId);
        
        trackingLogs.ForEach(log => log.TrackingLogEntriesStatuses =
                                                   trackingLogStatuses.Where(status =>
                                                       status.TrackingLogId == log.Id).ToList());
        return trackingLogs;
    }

    private async Task<List<TrackingLogEntryStatus>> GetAllTrackingLogsStatusesByUserId(SqlConnection connection, int userId)
    {
        var sql = @"select [S].[Id], [S].[TrackingLogId], [S].[Title], [S].[Description], [TL].[Id]
from [TrackingLog] as [TL] 
inner join [Status] as [S] on [S].[TrackingLogId] = [TL].[Id] 
where [TL].[CreatedBy] = @UserId";
				
        IEnumerable<TrackingLogEntryStatus> statuses = await connection.QueryAsync<TrackingLogEntryStatus,
                                                           TrackingLogGetResponse,
                                                           TrackingLogEntryStatus>(sql, (status, log) => {
                                                                                            status.TrackingLogId = log.Id;
                                                                                            return status;
                                                                                        },
                                                                                   splitOn: "Id",
                                                                                   param: new { UserId = userId });

        return statuses.ToList();
    }

    private async Task<List<TrackingLogGetResponse>> GetAllTrackingLogsWithEntriesByUserId(SqlConnection connection, int userId)
    {
        Dictionary<int, TrackingLogGetResponse> historyBoards = new();
        Dictionary<int, TrackingLogEntryGetResponse> historyTrackingLogEntries = new();
        Type[] types = {
                           typeof(TrackingLogGetResponse),
                           typeof(UserInfoDto),
                           typeof(UserInfoDto),
                           typeof(TrackingLogEntryGetResponse),
                           typeof(UserInfoDto),
                           typeof(UserInfoDto),
                           typeof(TrackingLogEntryStatus)
                       };

        IEnumerable<TrackingLogGetResponse> boards = await connection.QueryAsync<TrackingLogGetResponse>(
                                                          @"select [TL].[Id], [TL].[Title], [TL].[Description], [TL].[CreatedAt], [TL].[UpdatedAt], 
[TrackingLogCreator].[Id], [TrackingLogCreator].[UserName], [TrackingLogCreator].[FirstName], [TrackingLogCreator].[LastName], [TrackingLogCreator].[Email], 
[TrackingLogUpdater].[Id], [TrackingLogUpdater].[UserName], [TrackingLogUpdater].[FirstName], [TrackingLogUpdater].[LastName], [TrackingLogUpdater].[Email], 
[TLE].[Id], [TLE].[TrackingLogId], [TLE].[StatusId], [TLE].[Title], [TLE].[Description], [TLE].[CreatedAt], [TLE].[UpdatedAt], 
[TLECreator].[Id], [TLECreator].[UserName], [TLECreator].[FirstName], [TLECreator].[LastName], [TLECreator].[Email], 
[TLEUpdater].[Id], [TLEUpdater].[UserName], [TLEUpdater].[FirstName], [TLEUpdater].[LastName], [TLEUpdater].[Email], 
[S].[Id], [S].[Title], [S].[Description] 
from [TrackingLog] as [TL] 
inner join [User] as [TrackingLogCreator] on [TL].[CreatedBy] = [TrackingLogCreator].[Id] 
inner join [User] as [TrackingLogUpdater] on [TL].[UpdatedBy] = [TrackingLogUpdater].[Id] 
left join [TrackingLogEntry] as [TLE] on[TL].[Id] = [TLE].[TrackingLogId] 
left join [User] as [TLECreator] on [TLE].[CreatedBy] = [TLECreator].[Id] 
left join [User] as [TLEUpdater] on [TLE].[UpdatedBy] = [TLEUpdater].[Id] 
left join [Status] as [S] on [TLE].[StatusId] = [S].[Id] 
where [TrackingLogCreator].[Id] = @UserId",
                                                          types,
                                                          typesArg =>
                                                              ToDto(historyBoards,
                                                                    historyTrackingLogEntries,
                                                                    (TrackingLogGetResponse)typesArg[0],
                                                                    (UserInfoDto)typesArg[1],
                                                                    (UserInfoDto)typesArg[2],
                                                                    (TrackingLogEntryGetResponse?)typesArg[3],
                                                                    (UserInfoDto?)typesArg[4],
                                                                    (UserInfoDto?)typesArg[5],
                                                                    (TrackingLogEntryStatus?)typesArg[6]),
                                                          param: new { UserId = userId },
                                                          splitOn: "Id, Id, Id, Id, Id");

        return boards.Distinct().ToList();
    }

    private TrackingLogGetResponse ToDto(Dictionary<int, TrackingLogGetResponse> historyTrackingLogs, 
                                         Dictionary<int, TrackingLogEntryGetResponse> historyTrackingLogEntries, 
                                         TrackingLogGetResponse trackingLog, 
                                         UserInfoDto trackingLogCreator, 
                                         UserInfoDto trackingLogUpdater, 
                                         TrackingLogEntryGetResponse? trackingLogEntry, 
                                         UserInfoDto? trackingLogEntryCreator, 
                                         UserInfoDto? trackingLogEntryUpdater,
                                         TrackingLogEntryStatus? trackingLogEntryStatus)
    {
        if (!historyTrackingLogs.TryGetValue(trackingLog.Id, out TrackingLogGetResponse? curTrackingLog))
        {
            curTrackingLog = trackingLog;
            historyTrackingLogs.Add(curTrackingLog.Id, curTrackingLog);
            curTrackingLog.CreatedBy = trackingLogCreator;
            curTrackingLog.UpdatedBy = trackingLogUpdater;
            curTrackingLog.TrackingLogEntries = new();
        }

        if (trackingLogEntry == null)
        {
            return curTrackingLog;
        }

        if (!historyTrackingLogEntries.TryGetValue(trackingLogEntry.Id,
                                                   out TrackingLogEntryGetResponse? curTrackingLogEntry))
        {
            curTrackingLogEntry = trackingLogEntry;
            historyTrackingLogEntries.Add(curTrackingLogEntry.Id, curTrackingLogEntry);
            curTrackingLog.TrackingLogEntries.Add(curTrackingLogEntry);
            curTrackingLogEntry.CreatedBy = trackingLogEntryCreator!;
            curTrackingLogEntry.UpdatedBy = trackingLogEntryUpdater!;
        }

        if (trackingLogEntryStatus != null)
        {
            curTrackingLogEntry.Status = trackingLogEntryStatus;
        }

        return curTrackingLog;
    }

    #endregion

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert)
    {
        await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

        int id = await connection.ExecuteScalarAsync<int>(
                                                          @"insert into [Status] (Title, Description, TrackingLogId, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) values 
(@Title, @Description, @TrackingLogId, @CreatedById, @CreatedAt, @CreatedById, @CreatedAt); 

select scope_identity();",
                                                          statusToInsert);

        return await GetTrackingLogEntryStatusByIdInternal(connection, id);
    }

    private async Task<TrackingLogEntryStatus?> GetTrackingLogEntryStatusByIdInternal(SqlConnection connection, int id)
    {
        try
        {
            return await connection.QuerySingleAsync<TrackingLogEntryStatus>(@"SELECT [S].[ID], [S].[TrackingLogId], 
[S].[Title], [S].[Description]
FROM [Status] as [S]
WHERE [S].[Id] = @statusId",
                                                                             param: new { StatusId = id });
        }
        catch (Exception e)
        {
            return null;
        }
    }

    #endregion
}