namespace TaskManagerBackend.Application.Features.User.Dtos;

public class UpdatePasswordRequest
{
    public int UserId { get; init; }
    public required string OldPassword { get; init; }
    public required string NewPassword { get; init; }
}