namespace TaskManagerBackend.Dto.Board;

public class BoardCreateRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}