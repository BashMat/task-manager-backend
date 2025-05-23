#region Usings

using TaskManagerBackend.Common.Services;

#endregion

namespace TaskManagerBackend.Domain.Users;

public class NewUser
{
    public string UserName { get; }
    public string Email { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public byte[] PasswordHash { get; }
    public byte[] PasswordSalt { get; }

    public NewUser(IDateTimeService dateTimeService,
                   string userName,
                   string email,
                   byte[] passwordHash,
                   byte[] passwordSalt)
    {
        DateTime utcNow = dateTimeService.UtcNow;

        UserName = userName;
        Email = email;
        CreatedAt = utcNow;
        UpdatedAt = utcNow;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
}