﻿namespace TaskManagerBackend.Dto.Column
{
	public class ColumnInsertDto
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
        public int BoardId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
	}
}