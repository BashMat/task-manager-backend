namespace TaskManagerBackend.Common.Services;

/// <summary>
///     Represents service for executing cryptography operations.
/// </summary>
public interface ICryptographyService
{
    ValueTuple<byte[], byte[]> CreatePasswordHashAndSalt(string password);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string CreateToken(int userId);
}