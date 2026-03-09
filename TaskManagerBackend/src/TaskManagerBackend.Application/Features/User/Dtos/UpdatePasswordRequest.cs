#region Usings

using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

#endregion

namespace TaskManagerBackend.Application.Features.User.Dtos;

public class UpdatePasswordRequest
{
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int UserId { get; init; }
    
    [Password]
    public required string OldPassword { get; init; }
    
    [Password]
    public required string NewPassword { get; init; }
}