using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.User;

public class UserSignUpRequest
{
    [MaxLength(256)] 
    public required string UserName { get; init; }
    
    [MaxLength(256)]
    [EmailAddress]
    public required string Email { get; init; }

    // TODO: Create custom attribute with length and regex
    [MinLength(8)]
    public required string Password { get; init; }
}