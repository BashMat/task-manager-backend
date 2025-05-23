#region Usings

using TaskManagerBackend.Common.Services;

#endregion

namespace TaskManagerBackend.Domain.Users;

public class User
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public User(IDateTimeService dateTimeService,
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