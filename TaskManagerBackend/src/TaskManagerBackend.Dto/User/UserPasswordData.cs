namespace TaskManagerBackend.Dto.User;

public class UserPasswordData
{
    public int Id { get; set; }
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
}