namespace TaskManagerApi.Domain.Dtos.Board
{
	public class BoardUpdateRequestDto
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
    }
}
