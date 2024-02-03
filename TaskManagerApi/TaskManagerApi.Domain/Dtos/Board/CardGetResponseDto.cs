using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.Domain.Dtos.Board
{
	public class CardGetResponseDto
	{
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ColumnId { get; set; } = new();
        public Models.User CreatedBy { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public Models.User UpdatedBy { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }
}
