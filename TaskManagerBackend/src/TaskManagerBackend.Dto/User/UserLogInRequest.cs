using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.User;

public class UserLogInRequest
{
    // UserName or Email
    [MaxLength(256)]
    public required string LogInData { get; init; }

    // TODO: Create custom attribute with length and regex
    [MinLength(8)]
    public required string Password { get; init; }
}