using TaskManagerApi.Domain.Dtos.Board;
using TaskManagerApi.Domain;

namespace TaskManagerApi.Services.Board
{
	public interface IBoardService
	{
		public Task<ServiceResponse<List<BoardGetResponseDto>>> GetAll(int userId);

		public Task<ServiceResponse<BoardGetResponseDto>> GetById(int id);

		public Task<ServiceResponse<BoardGetResponseDto>> Create(int userId, BoardCreateRequestDto newBoard);

		public Task<ServiceResponse<BoardGetResponseDto>> Update(int userId, int boardId, BoardUpdateRequestDto updatedBoard);

		public Task<ServiceResponse<List<BoardGetResponseDto>>> Delete(int userId, int boardId);
	}
}
