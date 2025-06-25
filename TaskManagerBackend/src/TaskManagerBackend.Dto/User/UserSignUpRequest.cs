using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.User;

public class UserSignUpRequest
{
    [MaxLength(256)] 
    public required string UserName { get; init; }
    
    [MaxLength(256)]
    [EmailAddress]
    public required string Email { get; init; }
    
    // TODO: Make min length 8, do not forget to update integration tests
    [MinLength(4)]
    public required string Password { get; init; }
}