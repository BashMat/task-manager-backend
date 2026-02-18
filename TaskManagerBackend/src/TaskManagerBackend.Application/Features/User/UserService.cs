#region Usings

using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Application.Features.User;

public class UserService : IUserService
{
    public const string AccessDeniedMessage = "Cannot see this User profile";
    
    private readonly IUserRepository _userRepository;
    private readonly ICryptographyService _cryptographyService;
    private readonly IDateTimeService _dateTimeService;
    
    public UserService(IUserRepository userRepository,
                       ICryptographyService cryptographyService,
                       IDateTimeService dateTimeService) 
    {
        _userRepository = userRepository;
        _cryptographyService = cryptographyService;
        _dateTimeService = dateTimeService;
    }
    
    public async Task<ServiceResponse<GetUserDataResponse>> GetUserDataById(int currentUserId, 
                                                                            int userId)
    {
        // TODO: Implement better privacy options
        if (userId != currentUserId)
        {
            return new ServiceResponse<GetUserDataResponse>(actionResult: ActionResults.AccessDenied,
                                                            message: AccessDeniedMessage);
        }
        
        MinimalUserData? user = await _userRepository.GetMinimalUserData(userId);

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

        UserPasswordData? currentPasswordData = await _userRepository.GetUserPasswordData(request.UserId);

        if (currentPasswordData is null)
        {
            // TODO: Use better message and result type
            return new ServiceResponse<bool>(false,
                                             ActionResults.AccessDenied,
                                             AccessDeniedMessage);
        }

        if (!_cryptographyService.VerifyPasswordHash(request.OldPassword, currentPasswordData.PasswordHash, currentPasswordData.PasswordSalt))
        {
            // TODO: Use better message and result type
            return new ServiceResponse<bool>(false,
                                             ActionResults.AccessDenied,
                                             AccessDeniedMessage);
        }
        
        (byte[] newPasswordHash, byte[] newPasswordSalt) =
            _cryptographyService.CreatePasswordHashAndSalt(request.NewPassword);
        
        UserPasswordData newPasswordData = new(request.UserId, newPasswordHash, newPasswordSalt);
        bool isUpdateSuccessful = await _userRepository.UpdatePasswordData(newPasswordData, 
                                                                           _dateTimeService.UtcNow);

        if (isUpdateSuccessful)
        {
            return new ServiceResponse<bool>(true);
        }
        
        return new ServiceResponse<bool>(false,
                                         ActionResults.ServerError,
                                         "Unexpected error occurred. Try again later or contact support.");
    }
}