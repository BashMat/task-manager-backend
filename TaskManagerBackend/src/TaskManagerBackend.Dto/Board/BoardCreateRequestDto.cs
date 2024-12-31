namespace TaskManagerBackend.Dto.Board;

public class BoardCreateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}