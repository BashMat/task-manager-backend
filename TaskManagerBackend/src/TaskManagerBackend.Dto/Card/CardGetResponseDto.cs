#region Usings

using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Dto.Card;

public class CardGetResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ColumnId { get; set; }
    public int OrderIndex { get; set; }
    public UserInfoDto CreatedBy { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public UserInfoDto UpdatedBy { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}