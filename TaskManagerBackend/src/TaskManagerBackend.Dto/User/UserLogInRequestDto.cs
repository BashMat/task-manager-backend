namespace TaskManagerBackend.Dto.User;

public class UserLogInRequestDto
{
    // UserName or Email
    public string LogInData { get; set; } = null!;
    public string Password { get; set; } = null!;
}