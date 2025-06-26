using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserLogInRequest
{
    // UserName or Email
    [MaxLength(Constants.MaxDefaultTextLength)]
    public required string LogInData { get; init; }

    [Password]
    public required string Password { get; init; }
}