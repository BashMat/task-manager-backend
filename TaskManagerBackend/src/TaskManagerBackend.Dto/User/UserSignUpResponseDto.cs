namespace TaskManagerBackend.Dto.User;

public class UserSignUpResponseDto
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
}