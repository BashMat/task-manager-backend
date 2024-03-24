using TaskManagerApi.Dto.Board;
using TaskManagerApi.Dto.Card;
using TaskManagerApi.Dto.Column;

namespace TaskManagerApi.DataAccess.Repositories.Board
{
    public interface IBoardRepository
    {
        #region Board

        Task<BoardGetResponseDto?> Insert(BoardInsertDto insertedBoard);
        Task<List<BoardGetResponseDto>> GetAll(int userId);
        Task<BoardGetResponseDto?> GetById(int boardId);
        Task<BoardGetResponseDto?> Update(BoardUpdateDto updatedBoard);
        Task<List<BoardGetResponseDto>> Delete(int userId, int boardId);

        #endregion

        #region Column

        Task<ColumnGetResponseDto?> InsertColumn(ColumnInsertDto insertedColumn);
        Task<List<ColumnGetResponseDto>> GetAllColumns(int userId);
        Task<ColumnGetResponseDto?> GetColumnById(int columnId);
        Task<ColumnGetResponseDto?> UpdateColumn(ColumnUpdateDto updatedColumn);
        Task<List<ColumnGetResponseDto>> DeleteColumn(int userId, int columnId);

        #endregion

        #region Card

        Task<CardGetResponseDto?> InsertCard(CardInsertDto insertedCard);
        Task<List<CardGetResponseDto>> GetAllCards(int userId);
        Task<CardGetResponseDto?> GetCardById(int cardId);
        Task<CardGetResponseDto?> UpdateCard(CardUpdateDto updatedCard);
        Task<List<CardGetResponseDto>> DeleteCard(int userId, int cardId);

        #endregion
    }
}
