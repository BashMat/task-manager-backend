#region Usings

using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.User;

public interface IUserService
{
    Task<ServiceResponse<GetUserDataResponse>> GetUserDataById(int currentUserId, int userId);
}