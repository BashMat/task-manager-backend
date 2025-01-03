namespace TaskManagerBackend.Dto.Column;

public class ColumnCreateRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int BoardId { get; set; }
}