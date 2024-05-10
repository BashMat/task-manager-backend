namespace TaskManagerBackend.Dto.Column
{
	public class ColumnUpdateDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
	}
}
