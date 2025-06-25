namespace TaskManagerBackend.Domain.Users;

public class UserPasswordData
{
    public int Id { get; init; }
    public required byte[] PasswordHash { get; init; }
    public required byte[] PasswordSalt { get; init; }
}