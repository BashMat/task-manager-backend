namespace TaskManagerApi.Dto.Board
{
	public class CardCreateRequestDto
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public int ColumnId { get; set; }
    }
}
