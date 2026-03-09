#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

public interface IAuthService
{
    Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest request, CancellationToken cancellationToken);
    Task RevokeTokens(int userId, CancellationToken cancellationToken);
}