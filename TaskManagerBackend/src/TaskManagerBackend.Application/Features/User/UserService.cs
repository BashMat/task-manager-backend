#region Usings

using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Features.Users;
using TaskManagerBackend.Domain.Shared.Workflow;

#endregion

namespace TaskManagerBackend.Application.Features.User;

public class UserService(IUserRepository userRepository,
                         ICryptographyService cryptographyService,
                         IDateTimeService dateTimeService) : IUserService
{
    

    public async Task<ServiceResponse<GetUserDataResponse>> GetUserDataById(int currentUserId,
                                                                            int userId,
                                                                            CancellationToken cancellationToken)
    {
        // TODO: Implement better privacy options
        if (userId != currentUserId)
        {
            return new ServiceResponse<GetUserDataResponse>(actionResultType: ActionResultType.Unauthorized,
                                                            message: MessageResources.AccessDeniedMessage);
        }
        
        MinimalUserData? user = await userRepository.GetMinimalUserData(userId, cancellationToken);

        if (user is null)
        {
            return new ServiceResponse<GetUserDataResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                            message: MessageResources.ResourceDoesNotExist);
        }

        return new ServiceResponse<GetUserDataResponse>(new GetUserDataResponse
                                                        {
                                                            Id = user.Id,
                                                            UserName = user.Usernames.AccountName.Value
                                                        });
    }
    
    public async Task<ServiceResponse<bool>> UpdatePassword(int currentUserId, UpdatePasswordRequest request,
                                                            CancellationToken cancellationToken)
    {
        if (currentUserId != request.UserId)
        {
            return new ServiceResponse<bool>(false,
                                             ActionResultType.Unauthorized,
                                             MessageResources.AccessDeniedMessage);
        }

        UserPasswordData? currentPasswordData = await userRepository.GetUserPasswordData(request.UserId, cancellationToken);

        if (currentPasswordData is null)
        {
            // TODO: Use better message and result type
            return new ServiceResponse<bool>(false,
                                             ActionResultType.Unauthorized,
                                             MessageResources.AccessDeniedMessage);
        }

        if (!cryptographyService.VerifyPasswordHash(request.OldPassword, currentPasswordData.PasswordHash, currentPasswordData.PasswordSalt))
        {
            // TODO: Use better message and result type
            return new ServiceResponse<bool>(false,
                                             ActionResultType.Unauthorized,
                                             MessageResources.AccessDeniedMessage);
        }
        
        (byte[] newPasswordHash, byte[] newPasswordSalt) =
            cryptographyService.CreatePasswordHashAndSalt(request.NewPassword);
        
        UserPasswordData newPasswordData = new(request.UserId, newPasswordHash, newPasswordSalt);
        bool isUpdateSuccessful = await userRepository.UpdatePasswordData(newPasswordData, 
                                                                           dateTimeService.UtcNow, cancellationToken);

        if (isUpdateSuccessful)
        {
            return new ServiceResponse<bool>(true);
        }
        
        return new ServiceResponse<bool>(false,
                                         ActionResultType.ServerError,
                                         "Unexpected error occurred. Try again later or contact support.");
    }
}