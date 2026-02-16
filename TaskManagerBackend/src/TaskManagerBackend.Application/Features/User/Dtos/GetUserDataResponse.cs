namespace TaskManagerBackend.Application.Features.User.Dtos;

public class GetUserDataResponse
{
    public int Id { get; init; }
    public string UserName { get; init; } = null!;
}