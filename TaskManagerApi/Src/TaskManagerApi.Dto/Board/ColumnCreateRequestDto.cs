namespace TaskManagerApi.Dto.Board
{
	public class ColumnCreateRequestDto
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public int BoardId { get; set; }
	}
}
