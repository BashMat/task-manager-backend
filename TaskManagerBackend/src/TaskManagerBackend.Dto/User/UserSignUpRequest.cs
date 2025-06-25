using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Dto.User;

public class UserSignUpRequest
{
    [MaxLength(256)] 
    public required string UserName { get; init; }
    
    [MaxLength(256)]
    [EmailAddress]
    public required string Email { get; init; }

    [Password]
    public required string Password { get; init; }
}