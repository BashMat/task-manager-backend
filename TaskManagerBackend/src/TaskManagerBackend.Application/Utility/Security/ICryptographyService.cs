using Microsoft.IdentityModel.Tokens;

namespace TaskManagerBackend.Application.Utility.Security;

/// <summary>
///     Represents service for executing cryptography operations.
/// </summary>
public interface ICryptographyService
{
    ValueTuple<byte[], byte[]> CreatePasswordHashAndSalt(string password);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string CreateToken(int userId);
    SecurityKey GetSigningKey();
}