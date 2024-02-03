﻿namespace TaskManagerApi.Domain.Models
{
	public class BoardUpdateDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
	}
}
