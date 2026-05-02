namespace TaskManagerBackend.Domain.Features.Users;

public sealed class UserPasswordData
{
    // TODO: Process null values
    public UserPasswordData(int userId,
                            byte[] passwordHash,
                            byte[] passwordSalt)
    {
        UserId = userId;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
    
    public int UserId { get; init; }
    public byte[] PasswordHash { get; init; }
    public byte[] PasswordSalt { get; init; }
}