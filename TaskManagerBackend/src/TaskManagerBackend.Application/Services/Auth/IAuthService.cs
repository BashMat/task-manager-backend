#region Usings

using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Application.Services.Auth;

public interface IAuthService
{
    public Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest requestData);

    public Task<ServiceResponse<string>> LogIn(UserLogInRequest requestData);
}