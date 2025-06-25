namespace TaskManagerBackend.Dto.User;

public class UserSignUpResponse
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
}