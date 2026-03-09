#region Usings

using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Application.Features.User;

public class UserService(IUserRepository userRepository,
                         ICryptographyService cryptographyService,
                         IDateTimeService dateTimeService) : IUserService
{
    public const string AccessDeniedMessage = "Cannot see this User profile";

    public async Task<ServiceResponse<GetUserDataResponse>> GetUserDataById(int currentUserId, 
                                                                            int userId)
    {
        // TODO: Implement better privacy options
        if (userId != currentUserId)
        {
            return new ServiceResponse<GetUserDataResponse>(actionResult: ActionResults.AccessDenied,
                                                            message: AccessDeniedMessage);
        }
        
        MinimalUserData? user = await userRepository.GetMinimalUserData(userId);

        if (user is null)
        {
            return new ServiceResponse<GetUserDataResponse>(actionResult: ActionResults.ResourceNotFound,
                                                            message: $"User with id {userId} not found");
        }

        return new ServiceResponse<GetUserDataResponse>(new GetUserDataResponse
                                                        {
                                                            Id = user.Id,
                                                            UserName = user.UserName
                                                        });
    }
    
    public async Task<ServiceResponse<bool>> UpdatePassword(int currentUserId, UpdatePasswordRequest request)
    {
        if (currentUserId != request.UserId)
        {
            return new ServiceResponse<bool>(false,
                                             ActionResults.AccessDenied,
                                             AccessDeniedMessage);
        }

        UserPasswordData? currentPasswordData = await userRepository.GetUserPasswordData(request.UserId);

        if (currentPasswordData is null)
        {
            // TODO: Use better message and result type
            return new ServiceResponse<bool>(false,
                                             ActionResults.AccessDenied,
                                             AccessDeniedMessage);
        }

        if (!cryptographyService.VerifyPasswordHash(request.OldPassword, currentPasswordData.PasswordHash, currentPasswordData.PasswordSalt))
        {
            // TODO: Use better message and result type
            return new ServiceResponse<bool>(false,
                                             ActionResults.AccessDenied,
                                             AccessDeniedMessage);
        }
        
        (byte[] newPasswordHash, byte[] newPasswordSalt) =
            cryptographyService.CreatePasswordHashAndSalt(request.NewPassword);
        
        UserPasswordData newPasswordData = new(request.UserId, newPasswordHash, newPasswordSalt);
        bool isUpdateSuccessful = await userRepository.UpdatePasswordData(newPasswordData, 
                                                                           dateTimeService.UtcNow);

        if (isUpdateSuccessful)
        {
            return new ServiceResponse<bool>(true);
        }
        
        return new ServiceResponse<bool>(false,
                                         ActionResults.ServerError,
                                         "Unexpected error occurred. Try again later or contact support.");
    }
}