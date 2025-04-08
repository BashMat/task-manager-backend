using TaskManagerBackend.Dto.Tracking.TrackingLog;

namespace TaskManagerBackend.Application.Services.Tracking;

public interface ITrackingService
{
    #region Tracking Logs

    Task<ServiceResponse<TrackingLogGetResponse>> Create(int userId, TrackingLogCreateRequest newLog);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAll(int userId);
    Task<ServiceResponse<TrackingLogGetResponse>> GetById(int id);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> Delete(int userId, int boardId);

    #endregion
}