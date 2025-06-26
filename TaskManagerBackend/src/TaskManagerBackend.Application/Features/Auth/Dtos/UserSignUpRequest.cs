using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserSignUpRequest
{
    [MaxLength(Constants.MaxDefaultTextLength)] 
    public required string UserName { get; init; }
    
    [MaxLength(Constants.MaxDefaultTextLength)]
    [EmailAddress]
    public required string Email { get; init; }

    [Password]
    public required string Password { get; init; }
}