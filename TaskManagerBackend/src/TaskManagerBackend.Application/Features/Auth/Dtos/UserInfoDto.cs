namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserInfoDto
{
    public int Id { get; init; }
    public required string UserName { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public required string Email { get; init; }
}