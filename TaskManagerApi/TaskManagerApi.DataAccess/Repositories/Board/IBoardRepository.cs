using TaskManagerApi.Domain.Dtos.Board;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.DataAccess.Repositories.Board
{
    public interface IBoardRepository
    {
        Task<BoardGetResponseDto> Insert(BoardInsertDto insertedBoard);
        Task<List<BoardGetResponseDto>> GetAll(int userId);
        Task<BoardGetResponseDto?> GetById(int boardId);
        Task<BoardGetResponseDto> Update(BoardUpdateDto updatedBoard);
        Task<List<BoardGetResponseDto>> Delete(int userId, int boardId);
    }
}
