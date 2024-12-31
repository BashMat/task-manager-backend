namespace TaskManagerBackend.Dto.Column;

public class ColumnUpdateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}