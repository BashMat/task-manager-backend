using TaskManagerApi.Domain;
using TaskManagerApi.DataAccess.Repositories.Board;
using TaskManagerApi.Dto.Board;
using TaskManagerApi.Dto.Card;
using TaskManagerApi.Dto.Column;

namespace TaskManagerApi.Services.Board
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        #region Board

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

        #endregion

        #region Column

        public async Task<ServiceResponse<ColumnGetResponseDto>> CreateColumn(int userId, ColumnCreateRequestDto newColumn)
        {
            ColumnInsertDto columnInsert = new()
            {
                Title = newColumn.Title,
                Description = newColumn.Description,
                BoardId = newColumn.BoardId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            ServiceResponse<ColumnGetResponseDto> response = new()
            {
                Data = await _boardRepository.InsertColumn(columnInsert)
            };

            return response;
        }

        public async Task<ServiceResponse<List<ColumnGetResponseDto>>> GetAllColumns(int userId)
        {
            ServiceResponse<List<ColumnGetResponseDto>> response = new()
            {
                Data = await _boardRepository.GetAllColumns(userId)
            };

            return response;
        }

        public async Task<ServiceResponse<ColumnGetResponseDto>> GetColumnById(int id)
        {
            ServiceResponse<ColumnGetResponseDto> response = new()
            {
                Data = await _boardRepository.GetColumnById(id)
            };
            return response;
        }

        public async Task<ServiceResponse<ColumnGetResponseDto>> UpdateColumn(int userId,
                                                                              int columnId,
                                                                              ColumnUpdateRequestDto updatedColumn)
        {
            ColumnUpdateDto updatedColumnDb = new()
            {
                Id = columnId,
                Title = updatedColumn.Title,
                Description = updatedColumn.Description,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId
            };

            ServiceResponse<ColumnGetResponseDto> response = new()
            {
                Data = await _boardRepository.UpdateColumn(updatedColumnDb)
            };

            return response;
        }

        public async Task<ServiceResponse<List<ColumnGetResponseDto>>> DeleteColumn(int userId, int columnId)
        {
            ServiceResponse<List<ColumnGetResponseDto>> response = new()
            {
                Data = await _boardRepository.DeleteColumn(userId, columnId)
            };

            return response;
        }

        #endregion

        #region Card

        public async Task<ServiceResponse<CardGetResponseDto>> CreateCard(int userId, CardCreateRequestDto newCard)
        {
            CardInsertDto cardInsert = new()
            {
                Title = newCard.Title,
                Description = newCard.Description,
                ColumnId = newCard.ColumnId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            ServiceResponse<CardGetResponseDto> response = new()
            {
                Data = await _boardRepository.InsertCard(cardInsert)
            };

            return response;
        }

        public async Task<ServiceResponse<List<CardGetResponseDto>>> GetAllCards(int userId)
        {
            ServiceResponse<List<CardGetResponseDto>> response = new()
            {
                Data = await _boardRepository.GetAllCards(userId)
            };

            return response;
        }

        public async Task<ServiceResponse<CardGetResponseDto>> GetCardById(int id)
        {
            ServiceResponse<CardGetResponseDto> response = new()
            {
                Data = await _boardRepository.GetCardById(id)
            };
            return response;
        }

        public async Task<ServiceResponse<CardGetResponseDto>> UpdateCard(int userId,
                                                                          int cardId,
                                                                          CardUpdateRequestDto updatedCard)
        {
            CardUpdateDto updatedCardDb = new()
            {
                Id = cardId,
                Title = updatedCard.Title,
                Description = updatedCard.Description,
                ColumnId = updatedCard.ColumnId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId
            };

            ServiceResponse<CardGetResponseDto> response = new()
            {
                Data = await _boardRepository.UpdateCard(updatedCardDb)
            };

            return response;
        }

        public async Task<ServiceResponse<List<CardGetResponseDto>>> DeleteCard(int userId, int cardId)
        {
            ServiceResponse<List<CardGetResponseDto>> response = new()
            {
                Data = await _boardRepository.DeleteCard(userId, cardId)
            };

            return response;
        }

        #endregion
    }
}
