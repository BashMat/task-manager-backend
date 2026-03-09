#region Usings

using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.User;

public interface IUserService
{
    Task<ServiceResponse<GetUserDataResponse>> GetUserDataById(int currentUserId,
                                                               int userId,
                                                               CancellationToken cancellationToken);
    Task<ServiceResponse<bool>> UpdatePassword(int currentUserId,
                                               UpdatePasswordRequest request,
                                               CancellationToken cancellationToken);
}