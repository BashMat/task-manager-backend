#region Usings

using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Data;

#endregion

namespace TaskManagerBackend.Domain.Users;

public class NewUser
{
    public NewUser(IDateTimeService dateTimeService,
                   Usernames usernames,
                   byte[] passwordHash,
                   byte[] passwordSalt)
    {
        DateTime utcNow = dateTimeService.UtcNow;

        Usernames = usernames;
        CreatedAt = utcNow;
        UpdatedAt = utcNow;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
    
    public Usernames Usernames { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public byte[] PasswordHash { get; }
    public byte[] PasswordSalt { get; }
}