namespace TaskManagerBackend.Dto.Board;

public class BoardUpdateRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}