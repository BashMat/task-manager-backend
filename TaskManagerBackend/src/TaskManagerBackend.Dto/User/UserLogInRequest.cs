using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Dto.User;

public class UserLogInRequest
{
    // UserName or Email
    [MaxLength(256)]
    public required string LogInData { get; init; }

    [Password]
    public required string Password { get; init; }
}