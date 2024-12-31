namespace TaskManagerBackend.Dto.Board;

public class BoardUpdateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}