using TaskManagerBackend.DataAccess.Repositories.Board;
using TaskManagerBackend.Dto.Board;
using TaskManagerBackend.Dto.Card;
using TaskManagerBackend.Dto.Column;

namespace TaskManagerBackend.Application.Services.Board
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        
        private const string CouldNotCreateMessage = "Could not create resource";
        private const string ResourceDoesNotExist = "Resource does not exist";

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

            if (response.Data == null)
            {
                response.Success = false;
                response.Message = CouldNotCreateMessage;
            }

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
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = ResourceDoesNotExist;
            }

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
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = ResourceDoesNotExist;
            }

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
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = CouldNotCreateMessage;
            }

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
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = ResourceDoesNotExist;
            }

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

            if (response.Data == null)
            {
                response.Success = false;
                response.Message = ResourceDoesNotExist;
            }
            
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
                OrderIndex = newCard.OrderIndex,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            ServiceResponse<CardGetResponseDto> response = new()
            {
                Data = await _boardRepository.InsertCard(cardInsert)
            };
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = CouldNotCreateMessage;
            }

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
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = ResourceDoesNotExist;
            }
            
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
                OrderIndex = updatedCard.OrderIndex,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId
            };

            ServiceResponse<CardGetResponseDto> response = new()
            {
                Data = await _boardRepository.UpdateCard(updatedCardDb)
            };
            
            if (response.Data == null)
            {
                response.Success = false;
                response.Message = ResourceDoesNotExist;
            }

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
