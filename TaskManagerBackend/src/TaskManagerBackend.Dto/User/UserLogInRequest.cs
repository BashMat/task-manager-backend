namespace TaskManagerBackend.Dto.User;

public class UserLogInRequest
{
    // UserName or Email
    public required string LogInData { get; init; }
    public required string Password { get; init; }
}