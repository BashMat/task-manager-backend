#region Usings

using TaskManagerBackend.Common;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Application.Services.Auth;

public interface IAuthService
{
    public Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData);

    public Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData);
}