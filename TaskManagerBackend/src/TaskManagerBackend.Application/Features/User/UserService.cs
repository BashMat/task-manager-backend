#region Usings

using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Application.Features.User;

public class UserService : IUserService
{
    public const string AccessDeniedMessage = "Cannot see this User profile";
    
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<ServiceResponse<GetUserDataResponse>> GetUserDataById(int currentUserId, 
                                                                            int userId)
    {
        // TODO: Implement better privacy options
        if (userId != currentUserId)
        {
            return new ServiceResponse<GetUserDataResponse>(actionResult: ActionResults.AccessDenied,
                                                            message: "Cannot see this User profile");
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
}