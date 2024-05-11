namespace TaskManagerBackend.Application.Services.Auth;

public interface IAuthProvider
{
    ValueTuple<byte[], byte[]> CreatePasswordHashAndSalt(string password);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    string CreateToken(int userId);
}