namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserSignUpResponse
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
}