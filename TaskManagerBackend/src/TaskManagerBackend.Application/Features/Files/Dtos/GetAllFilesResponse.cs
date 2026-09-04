namespace TaskManagerBackend.Application.Features.Files.Dtos;

public class GetAllFilesResponse
{
    public required IReadOnlyCollection<string> FileNames { get; init; }
}