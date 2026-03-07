#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

public interface IAuthService
{
    Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest requestData);
    Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest requestData);
    Task RevokeTokens(int userId);
}