#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

public interface IAuthService
{
    public Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest requestData);

    [Obsolete($"{nameof(IssueToken)} method must be used to issue both Access and Refresh tokens")]
    public Task<ServiceResponse<string>> LogIn(UserLogInRequest requestData);
    
    public Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest requestData);
}