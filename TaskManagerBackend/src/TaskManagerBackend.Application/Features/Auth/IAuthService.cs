#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

public interface IAuthService
{
    Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest requestData);

    [Obsolete($"{nameof(IssueToken)} method must be used to issue both Access and Refresh tokens")]
    Task<ServiceResponse<string>> LogIn(UserLogInRequest requestData);
    
    Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest requestData);
    Task RevokeTokens(int userId);
}