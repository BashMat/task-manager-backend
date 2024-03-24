﻿using TaskManagerApi.Dto.Column;
using TaskManagerApi.Dto.User;

namespace TaskManagerApi.Dto.Board
{
	public class BoardGetResponseDto
	{
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public UserInfoDto CreatedBy { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public UserInfoDto UpdatedBy { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
        public List<ColumnGetResponseDto> Columns { get; set; } = new();
    }
}
