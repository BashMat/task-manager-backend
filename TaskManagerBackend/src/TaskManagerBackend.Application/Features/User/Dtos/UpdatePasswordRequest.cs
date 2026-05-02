#region Usings

using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Shared.Validation;

#endregion

namespace TaskManagerBackend.Application.Features.User.Dtos;

public class UpdatePasswordRequest
{
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int UserId { get; init; }
    
    [Required]
    [Password]
    public string OldPassword { get; init; } = null!;
    
    [Required]
    [Password]
    public string NewPassword { get; init; } = null!;
}