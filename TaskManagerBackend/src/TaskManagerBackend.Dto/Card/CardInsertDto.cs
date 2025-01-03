namespace TaskManagerBackend.Dto.Card;

public class CardInsertDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ColumnId { get; set; }
    public int OrderIndex { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}