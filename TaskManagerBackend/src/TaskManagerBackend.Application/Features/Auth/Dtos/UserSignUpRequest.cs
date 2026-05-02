using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Shared.Validation;

namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class UserSignUpRequest
{
    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)] 
    public string UserName { get; init; } = null!;
    
    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    [Password]
    public string Password { get; init; } = null!;
}