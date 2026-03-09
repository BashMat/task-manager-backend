namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserSignUpResponse
{
    public required int Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
}