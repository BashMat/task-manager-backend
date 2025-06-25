using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.User;

public class UserLogInRequest
{
    // UserName or Email
    [MaxLength(256)]
    public required string LogInData { get; init; }

    // TODO: Make min length 8, do not forget to update integration tests
    [MinLength(4)]
    public required string Password { get; init; }
}