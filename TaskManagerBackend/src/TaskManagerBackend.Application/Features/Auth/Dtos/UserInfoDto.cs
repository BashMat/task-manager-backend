namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserInfoDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
}