namespace TaskManagerBackend.Application.Features.User.Dtos;

public class GetUserDataResponse
{
    public int Id { get; init; }
    public required string UserName { get; init; }
}