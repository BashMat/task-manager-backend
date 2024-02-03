using AutoMapper;
using TaskManagerApi.Domain.Dtos.Board;
using TaskManagerApi.Domain;
using TaskManagerApi.DataAccess.Repositories.Board;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.Services.Board
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public async Task<ServiceResponse<List<BoardGetResponseDto>>> GetAll(int userId)
        {
            ServiceResponse<List<BoardGetResponseDto>> response = new()
            {
                Data = await _boardRepository.GetAll(userId)
            };

            return response;
        }

        public async Task<ServiceResponse<BoardGetResponseDto>> GetById(int id)
        {
            ServiceResponse<BoardGetResponseDto> response = new()
            {
                Data = await _boardRepository.GetById(id)
            };
            return response;
        }

        public async Task<ServiceResponse<BoardGetResponseDto>> Create(int userId, BoardCreateRequestDto newBoard)
        {
            BoardInsertDto boardInsert = new()
            {
                Title = newBoard.Title,
                Description = newBoard.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            ServiceResponse<BoardGetResponseDto> response = new()
            {
                Data = await _boardRepository.Insert(boardInsert)
            };

            return response;
        }

        public async Task<ServiceResponse<BoardGetResponseDto>> Update(int userId, int boardId, BoardUpdateRequestDto updatedBoard)
        {
            BoardUpdateDto updatedBoardDb = new()
            {
                Id = boardId,
                Title = updatedBoard.Title,
                Description = updatedBoard.Description,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId
            };

            ServiceResponse<BoardGetResponseDto> response = new()
            {
                Data = await _boardRepository.Update(updatedBoardDb)
            };

            return response;
        }

        public async Task<ServiceResponse<List<BoardGetResponseDto>>> Delete(int userId, int boardId)
        {
            ServiceResponse<List<BoardGetResponseDto>> response = new()
            {
                Data = await _boardRepository.Delete(userId, boardId)
            };

            return response;
        }
    }
}
