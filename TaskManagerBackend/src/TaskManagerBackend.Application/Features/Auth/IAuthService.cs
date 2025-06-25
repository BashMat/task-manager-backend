#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

public interface IAuthService
{
    public Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest requestData);

    public Task<ServiceResponse<string>> LogIn(UserLogInRequest requestData);
}