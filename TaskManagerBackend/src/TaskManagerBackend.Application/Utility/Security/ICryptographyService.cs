using Microsoft.IdentityModel.Tokens;
using TaskManagerBackend.Domain.Users;

namespace TaskManagerBackend.Application.Utility.Security;

/// <summary>
///     Represents service for executing cryptography operations.
/// </summary>
public interface ICryptographyService
{
    ValueTuple<byte[], byte[]> CreatePasswordHashAndSalt(string password);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string IssueAccessToken(int userId);
    RefreshTokenData IssueRefreshToken(int userId);
    TokenValidationParameters GetValidationParameters();
    int? GetUserIdOrNull(string token);
    Guid? GetTokenIdOrNull(string token);
    bool VerifyToken(string token);
}