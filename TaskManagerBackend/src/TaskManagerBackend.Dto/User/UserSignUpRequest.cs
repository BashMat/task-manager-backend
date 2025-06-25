using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.User;

public class UserSignUpRequest
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}