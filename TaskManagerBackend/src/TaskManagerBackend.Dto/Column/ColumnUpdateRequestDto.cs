namespace TaskManagerBackend.Dto.Column;

public class ColumnUpdateRequestDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}