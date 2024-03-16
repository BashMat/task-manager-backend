namespace TaskManagerApi.Domain.Dtos.Project
{
	public class ProjectUpdateRequestDto
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;
	}
}
