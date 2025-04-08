﻿#region Usings

using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Repositories.Tracking;
using TaskManagerBackend.Dto.Tracking.TrackingLog;

#endregion

namespace TaskManagerBackend.Application.Services.Tracking;

public class TrackingService : ITrackingService
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IDateTimeService _dateTimeService;
        
    private const string CouldNotCreateMessage = "Could not create resource";
    private const string ResourceDoesNotExist = "Resource does not exist";

    public TrackingService(ITrackingRepository trackingRepository,
                           IDateTimeService dateTimeService)
    {
        _trackingRepository = trackingRepository;
        _dateTimeService = dateTimeService;
    }
    
    #region Tracking Logs
    
    public async Task<ServiceResponse<TrackingLogGetResponse>> Create(int userId, TrackingLogCreateRequest newLog)
    {
        NewTrackingLog logToInsert = new()
                                     {
                                         Title = newLog.Title,
                                         Description = newLog.Description,
                                         CreatedById = userId,
                                         CreatedAt = _dateTimeService.UtcNow
                                     };

        ServiceResponse<TrackingLogGetResponse> response = new()
                                                           {
                                                               Data = await _trackingRepository.Insert(logToInsert)
                                                           };

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = CouldNotCreateMessage;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAll(int userId)
    {
        ServiceResponse<List<TrackingLogGetResponse>> response = new()
                                                              {
                                                                  Data = await _trackingRepository.GetAll(userId)
                                                              };

        return response;
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> GetById(int id)
    {
        ServiceResponse<TrackingLogGetResponse> response = new()
                                                        {
                                                            Data = await _trackingRepository.GetById(id)
                                                        };
            
        if (response.Data == null)
        {
            response.Success = false;
            response.Message = ResourceDoesNotExist;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> Delete(int userId, int boardId)
    {
        ServiceResponse<List<TrackingLogGetResponse>> response = new()
                                                              {
                                                                  Data = await _trackingRepository.Delete(userId, boardId)
                                                              };

        return response;
    }

    #endregion
}