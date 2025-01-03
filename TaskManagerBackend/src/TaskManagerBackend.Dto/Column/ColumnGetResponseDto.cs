#region Usings

using TaskManagerBackend.Dto.Card;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Dto.Column;

public class ColumnGetResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int BoardId { get; set; }
    public UserInfoDto CreatedBy { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public UserInfoDto UpdatedBy { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
    public List<CardGetResponseDto> Cards { get; set; } = new();
}