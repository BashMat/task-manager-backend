#region Usings

using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Dto.Board;
using TaskManagerBackend.Dto.Card;
using TaskManagerBackend.Dto.Column;

#endregion

namespace TaskManagerBackend.Application.Services.Board;

public interface IBoardService
{
    #region Board

    public Task<ServiceResponse<BoardGetResponseDto>> Create(int userId, BoardCreateRequestDto newBoard);
    public Task<ServiceResponse<List<BoardGetResponseDto>>> GetAll(int userId);
    public Task<ServiceResponse<BoardGetResponseDto>> GetById(int id);
    public Task<ServiceResponse<BoardGetResponseDto>> Update(int userId, int boardId, BoardUpdateRequestDto updatedBoard);
    public Task<ServiceResponse<List<BoardGetResponseDto>>> Delete(int userId, int boardId);

    #endregion

    #region Column

    public Task<ServiceResponse<ColumnGetResponseDto>> CreateColumn(int userId, ColumnCreateRequestDto newColumn);
    public Task<ServiceResponse<List<ColumnGetResponseDto>>> GetAllColumns(int userId);
    public Task<ServiceResponse<ColumnGetResponseDto>> GetColumnById(int id);
    public Task<ServiceResponse<ColumnGetResponseDto>> UpdateColumn(int userId, int columnId, ColumnUpdateRequestDto updatedColumn);
    public Task<ServiceResponse<List<ColumnGetResponseDto>>> DeleteColumn(int userId, int columnId);

    #endregion

    #region Card
    public Task<ServiceResponse<CardGetResponseDto>> CreateCard(int userId, CardCreateRequestDto newCard);
    public Task<ServiceResponse<List<CardGetResponseDto>>> GetAllCards(int userId);
    public Task<ServiceResponse<CardGetResponseDto>> GetCardById(int id);
    public Task<ServiceResponse<CardGetResponseDto>> UpdateCard(int userId, int cardId, CardUpdateRequestDto updatedCard);
    public Task<ServiceResponse<List<CardGetResponseDto>>> DeleteCard(int userId, int cardId);

    #endregion
}