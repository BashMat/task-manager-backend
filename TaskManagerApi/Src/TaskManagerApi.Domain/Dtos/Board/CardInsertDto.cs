namespace TaskManagerApi.Domain.Models
{
	public class CardInsertDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public int ColumnId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
	}
}
