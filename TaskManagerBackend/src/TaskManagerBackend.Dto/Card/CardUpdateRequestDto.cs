namespace TaskManagerBackend.Dto.Card;

public class CardUpdateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ColumnId { get; set; }
    public int OrderIndex { get; set; }
}