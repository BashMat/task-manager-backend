using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagerBackend.Common;
using TaskManagerBackend.Dto.Board;
using TaskManagerBackend.Dto.Card;
using TaskManagerBackend.Dto.Column;
using TaskManagerBackend.Dto.User;

namespace TaskManagerBackend.DataAccess.Repositories.Board
{
    public class BoardRepository : IBoardRepository
    {
        private readonly IConfiguration _configuration;

        public BoardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Board

        public async Task<BoardGetResponseDto?> Insert(BoardInsertDto insertedBoard)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            int id = await connection.ExecuteScalarAsync<int>(
            @"insert into [Board] (Title, Description, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) values 
(@Title, @Description, @CreatedBy, @CreatedAt, @CreatedBy, @CreatedAt); 

select scope_identity();",
            insertedBoard);

            return await GetByIdInternal(connection, id);
        }

        public async Task<List<BoardGetResponseDto>> GetAll(int userId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            return await GetAllInternal(connection, userId);
        }

        public async Task<BoardGetResponseDto?> GetById(int boardId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            return await GetByIdInternal(connection, boardId);
        }

        public async Task<BoardGetResponseDto?> Update(BoardUpdateDto updatedBoard)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            await connection.ExecuteAsync(@"update [Board] 
set [Board].[Title] = @Title, [Board].[Description] = @Description, 
[Board].[UpdatedBy] = @UpdatedBy, [Board].[UpdatedAt] = @UpdatedAt 
where [Board].[Id] = @Id", updatedBoard);

            return await GetByIdInternal(connection, updatedBoard.Id);
        }

        public async Task<List<BoardGetResponseDto>> Delete(int userId, int boardId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            await connection.ExecuteAsync("delete from [Board] where [Board].[Id]=@BoardId",
                                          new { BoardId = boardId });

            return await GetAllInternal(connection, userId);
        }

        #region BoardInternal

        private async Task<BoardGetResponseDto?> GetByIdInternal(SqlConnection connection, int boardId)
        {
            Dictionary<int, BoardGetResponseDto> historyBoards = new();
            Dictionary<int, ColumnGetResponseDto> historyColumns = new();
            Type[] types = {
                typeof(BoardGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto),
                typeof(ColumnGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto),
                typeof(CardGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto)
            };

            IEnumerable<BoardGetResponseDto> boards = await connection.QueryAsync<BoardGetResponseDto>(
                @"select [B].[Id], [B].[Title], [B].[Description], [B].[CreatedAt], [B].[UpdatedAt], 
[BoardCreator].[Id], [BoardCreator].[UserName], [BoardCreator].[FirstName], [BoardCreator].[LastName], [BoardCreator].[Email], 
[BoardUpdater].[Id], [BoardUpdater].[UserName], [BoardUpdater].[FirstName], [BoardUpdater].[LastName], [BoardUpdater].[Email], 
[Col].[Id], [Col].[BoardId], [Col].[Title], [Col].[Description], [Col].[CreatedAt], [Col].[UpdatedAt], 
[ColumnCreator].[Id], [ColumnCreator].[UserName], [ColumnCreator].[FirstName], [ColumnCreator].[LastName], [ColumnCreator].[Email], 
[ColumnUpdater].[Id], [ColumnUpdater].[UserName], [ColumnUpdater].[FirstName], [ColumnUpdater].[LastName], [ColumnUpdater].[Email], 
[Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], [Card].[CreatedAt], [Card].[UpdatedAt], 
[CardCreator].[Id], [CardCreator].[UserName], [CardCreator].[FirstName], [CardCreator].[LastName], [CardCreator].[Email], 
[CardUpdater].[Id], [CardUpdater].[UserName], [CardUpdater].[FirstName], [CardUpdater].[LastName], [CardUpdater].[Email] 
from [Board] as [B] 
inner join [User] as [BoardCreator] on [B].[CreatedBy] = [BoardCreator].[Id] 
inner join [User] as [BoardUpdater] on [B].[UpdatedBy] = [BoardUpdater].[Id] 
left join [Column] as [Col] on [B].[Id] = [Col].[BoardId] 
left join [User] as [ColumnCreator] on [Col].[CreatedBy] = [ColumnCreator].[Id] 
left join [User] as [ColumnUpdater] on [Col].[UpdatedBy] = [ColumnUpdater].[Id] 
left join [Card] on [Col].[Id] = [Card].[ColumnId] 
left join [User] as [CardCreator] on [Card].[CreatedBy] = [CardCreator].[Id] 
left join [User] as [CardUpdater] on [Card].[UpdatedBy] = [CardUpdater].[Id] 
where [B].[Id] = @BoardId",
                types,
                typesArg =>
                ToDto(historyBoards,
                      historyColumns,
                      (BoardGetResponseDto)typesArg[0],
                      (UserInfoDto)typesArg[1],
                      (UserInfoDto)typesArg[2],
                      (ColumnGetResponseDto?)typesArg[3],
                      (UserInfoDto?)typesArg[4],
                      (UserInfoDto?)typesArg[5],
                      (CardGetResponseDto?)typesArg[6],
                      (UserInfoDto?)typesArg[7],
                      (UserInfoDto?)typesArg[8]),
                param: new { BoardId = boardId },
                splitOn: "Id, Id, Id, Id, Id, Id, Id, Id");

            var listBoards = boards.Distinct().ToList();
            return listBoards.Count == 0
                ? null 
                : listBoards[0];
        }

        private async Task<List<BoardGetResponseDto>> GetAllInternal(SqlConnection connection, int userId)
        {
            Dictionary<int, BoardGetResponseDto> historyBoards = new();
            Dictionary<int, ColumnGetResponseDto> historyColumns = new();
            Type[] types = new[]
            {
                typeof(BoardGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto),
                typeof(ColumnGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto),
                typeof(CardGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto)
            };

            IEnumerable<BoardGetResponseDto> boards = await connection.QueryAsync<BoardGetResponseDto>(
                @"select [B].[Id], [B].[Title], [B].[Description], [B].[CreatedAt], [B].[UpdatedAt], 
[BoardCreator].[Id], [BoardCreator].[UserName], [BoardCreator].[FirstName], [BoardCreator].[LastName], [BoardCreator].[Email], 
[BoardUpdater].[Id], [BoardUpdater].[UserName], [BoardUpdater].[FirstName], [BoardUpdater].[LastName], [BoardUpdater].[Email], 
[Col].[Id], [Col].[BoardId], [Col].[Title], [Col].[Description], [Col].[CreatedAt], [Col].[UpdatedAt], 
[ColumnCreator].[Id], [ColumnCreator].[UserName], [ColumnCreator].[FirstName], [ColumnCreator].[LastName], [ColumnCreator].[Email], 
[ColumnUpdater].[Id], [ColumnUpdater].[UserName], [ColumnUpdater].[FirstName], [ColumnUpdater].[LastName], [ColumnUpdater].[Email], 
[Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], [Card].[CreatedAt], [Card].[UpdatedAt], 
[CardCreator].[Id], [CardCreator].[UserName], [CardCreator].[FirstName], [CardCreator].[LastName], [CardCreator].[Email], 
[CardUpdater].[Id], [CardUpdater].[UserName], [CardUpdater].[FirstName], [CardUpdater].[LastName], [CardUpdater].[Email] 
from [Board] as [B] 
inner join [User] as [BoardCreator] on [B].[CreatedBy] = [BoardCreator].[Id] 
inner join [User] as [BoardUpdater] on [B].[UpdatedBy] = [BoardUpdater].[Id] 
left join [Column] as [Col] on[B].[Id] = [Col].[BoardId] 
left join [User] as [ColumnCreator] on [Col].[CreatedBy] = [ColumnCreator].[Id] 
left join [User] as [ColumnUpdater] on [Col].[UpdatedBy] = [ColumnUpdater].[Id] 
left join [Card] on[Col].[Id] = [Card].[ColumnId] 
left join [User] as [CardCreator] on [Card].[CreatedBy] = [CardCreator].[Id] 
left join [User] as [CardUpdater] on [Card].[UpdatedBy] = [CardUpdater].[Id] 
where [BoardCreator].[Id] = @UserId",
                types,
                typesArg =>
                ToDto(historyBoards,
                      historyColumns,
                      (BoardGetResponseDto)typesArg[0],
                      (UserInfoDto)typesArg[1],
                      (UserInfoDto)typesArg[2],
                      (ColumnGetResponseDto?)typesArg[3],
                      (UserInfoDto?)typesArg[4],
                      (UserInfoDto?)typesArg[5],
                      (CardGetResponseDto?)typesArg[6],
                      (UserInfoDto?)typesArg[7],
                      (UserInfoDto?)typesArg[8]),
                param: new { UserId = userId },
                splitOn: "Id, Id, Id, Id, Id, Id, Id, Id");
            return boards.Distinct().ToList();
        }

        private BoardGetResponseDto ToDto(Dictionary<int, BoardGetResponseDto> historyBoards,
                                          Dictionary<int, ColumnGetResponseDto> historyColumns,
                                          BoardGetResponseDto board,
                                          UserInfoDto boardCreator,
                                          UserInfoDto boardUpdater,
                                          ColumnGetResponseDto? column,
                                          UserInfoDto? columnCreator,
                                          UserInfoDto? columnUpdater,
                                          CardGetResponseDto? card,
                                          UserInfoDto? cardCreator,
                                          UserInfoDto? cardUpdater)
        {
            if (!historyBoards.TryGetValue(board.Id, out BoardGetResponseDto? curBoard))
            {
                curBoard = board;
                historyBoards.Add(curBoard.Id, curBoard);
                curBoard.CreatedBy = boardCreator;
                curBoard.UpdatedBy = boardUpdater;
                curBoard.Columns = new();
            }

            if (column == null)
            {
                return curBoard;
            }

            if (!historyColumns.TryGetValue(column.Id, out ColumnGetResponseDto? curColumn))
            {
                curColumn = column;
                historyColumns.Add(curColumn.Id, curColumn);
                curBoard.Columns.Add(curColumn);
                curColumn.CreatedBy = columnCreator!;
                curColumn.UpdatedBy = columnUpdater!;
                curColumn.Cards = new();
            }

            if (card != null)
            {
                curColumn.Cards.Add(card);
                card.CreatedBy = cardCreator!;
                card.UpdatedBy = cardUpdater!;
            }

            return curBoard;
        }

        #endregion

        #endregion

        #region Column

        public async Task<ColumnGetResponseDto?> InsertColumn(ColumnInsertDto insertedColumn)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            int id = await connection.ExecuteScalarAsync<int>("insert into [Column] ([Title], [Description], " +
                "[BoardId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt]) " +
                "values (@Title, @Description, @BoardId, @CreatedBy, @CreatedAt, @CreatedBy, @CreatedAt); " +
                "select scope_identity();",
                insertedColumn);

            return await GetColumnByIdInternal(connection, id);
        }

        public async Task<List<ColumnGetResponseDto>> GetAllColumns(int userId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            return await GetAllColumnsInternal(connection, userId);
        }

        public async Task<ColumnGetResponseDto?> GetColumnById(int columnId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            return await GetColumnByIdInternal(connection, columnId);
        }

        public async Task<ColumnGetResponseDto?> UpdateColumn(ColumnUpdateDto updatedColumn)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            await connection.ExecuteAsync(@"update [Column] 
set [Column].[Title] = @Title, [Column].[Description] = @Description, 
[Column].[UpdatedBy] = @UpdatedBy, [Column].[UpdatedAt] = @UpdatedAt 
where [Column].[Id] = @Id", 
                                          updatedColumn);

            return await GetColumnByIdInternal(connection, updatedColumn.Id);
        }

        public async Task<List<ColumnGetResponseDto>> DeleteColumn(int userId, int columnId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            await connection.ExecuteAsync("delete from [Column] where [Column].[Id] = @ColumnId",
                                          new { ColumnId = columnId });

            return await GetAllColumnsInternal(connection, userId);
        }

        #region ColumnInternal

        private async Task<List<ColumnGetResponseDto>> GetAllColumnsInternal(SqlConnection connection, int userId)
        {
            Dictionary<int, ColumnGetResponseDto> historyColumns = new();
            Type[] types = {
                typeof(ColumnGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto),
                typeof(CardGetResponseDto),
                typeof(UserInfoDto),
                typeof(UserInfoDto)
            };

            IEnumerable<ColumnGetResponseDto> columns = await connection.QueryAsync<ColumnGetResponseDto>(
@"select [Col].[Id], [Col].[BoardId], [Col].[Title], [Col].[Description], [Col].[CreatedAt], [Col].[UpdatedAt], 
[ColumnCreator].[Id], [ColumnCreator].[UserName], [ColumnCreator].[FirstName], [ColumnCreator].[LastName], [ColumnCreator].[Email], 
[ColumnUpdater].[Id], [ColumnUpdater].[UserName], [ColumnUpdater].[FirstName], [ColumnUpdater].[LastName], [ColumnUpdater].[Email], 
[Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], [Card].[CreatedAt], [Card].[UpdatedAt], 
[CardCreator].[Id], [CardCreator].[UserName], [CardCreator].[FirstName], [CardCreator].[LastName], [CardCreator].[Email], 
[CardUpdater].[Id], [CardUpdater].[UserName], [CardUpdater].[FirstName], [CardUpdater].[LastName], [CardUpdater].[Email] 
from [Column] as [Col] 
left join [User] as [ColumnCreator] on [Col].[CreatedBy] = [ColumnCreator].[Id] 
left join [User] as [ColumnUpdater] on [Col].[UpdatedBy] = [ColumnUpdater].[Id] 
left join [Card] on[Col].[Id] = [Card].[ColumnId] 
left join [User] as [CardCreator] on [Card].[CreatedBy] = [CardCreator].[Id] 
left join [User] as [CardUpdater] on [Card].[UpdatedBy] = [CardUpdater].[Id] 
where [ColumnCreator].[Id] = @UserId",
                types,
                typesArg =>
                ToDto(historyColumns,
                      (ColumnGetResponseDto)typesArg[0],
                      (UserInfoDto)typesArg[1],
                      (UserInfoDto)typesArg[2],
                      (CardGetResponseDto?)typesArg[3],
                      (UserInfoDto?)typesArg[4],
                      (UserInfoDto?)typesArg[5]),
                param: new { UserId = userId },
                splitOn: "Id, Id, Id, Id, Id");
            return columns.Distinct().ToList();
        }

        private async Task<ColumnGetResponseDto?> GetColumnByIdInternal(SqlConnection connection, int columnId)
        {
            Dictionary<int, ColumnGetResponseDto> historyColumns = new();

            IEnumerable<ColumnGetResponseDto> columns = await connection.QueryAsync<ColumnGetResponseDto,
                                                                                    UserInfoDto,
                                                                                    UserInfoDto,
                                                                                    CardGetResponseDto,
                                                                                    UserInfoDto,
                                                                                    UserInfoDto,
                                                                                    ColumnGetResponseDto>(
                @"select [Col].[Id], [Col].[BoardId], [Col].[Title], [Col].[Description], [Col].[CreatedAt], [Col].[UpdatedAt], 
[ColumnCreator].[Id], [ColumnCreator].[UserName], [ColumnCreator].[FirstName], [ColumnCreator].[LastName], [ColumnCreator].[Email], 
[ColumnUpdater].[Id], [ColumnUpdater].[UserName], [ColumnUpdater].[FirstName], [ColumnUpdater].[LastName], [ColumnUpdater].[Email], 
[Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], [Card].[CreatedAt], [Card].[UpdatedAt], 
[CardCreator].[Id], [CardCreator].[UserName], [CardCreator].[FirstName], [CardCreator].[LastName], [CardCreator].[Email], 
[CardUpdater].[Id], [CardUpdater].[UserName], [CardUpdater].[FirstName], [CardUpdater].[LastName], [CardUpdater].[Email] 
from [Column] as [Col] 
left join [User] as [ColumnCreator] on [Col].[CreatedBy] = [ColumnCreator].[Id] 
left join [User] as [ColumnUpdater] on [Col].[UpdatedBy] = [ColumnUpdater].[Id] 
left join [Card] on[Col].[Id] = [Card].[ColumnId] 
left join [User] as [CardCreator] on [Card].[CreatedBy] = [CardCreator].[Id] 
left join [User] as [CardUpdater] on [Card].[UpdatedBy] = [CardUpdater].[Id] 
where [Col].[Id] = @ColumnId",
                (column, columnCreator, columnUpdater, card, cardCreator, cardUpdater) =>
                ToDto(historyColumns,
                      column,
                      columnCreator,
                      columnUpdater,
                      card,
                      cardCreator,
                      cardUpdater),
                param: new { ColumnId = columnId },
                splitOn: "Id, Id, Id, Id, Id");
            
            var listColumns = columns.Distinct().ToList();
            return listColumns.Count == 0
                ? null 
                : listColumns[0];
        }

        private ColumnGetResponseDto ToDto(Dictionary<int, ColumnGetResponseDto> historyColumns,
                                           ColumnGetResponseDto column,
                                           UserInfoDto columnCreator,
                                           UserInfoDto columnUpdater,
                                           CardGetResponseDto? card,
                                           UserInfoDto? cardCreator,
                                           UserInfoDto? cardUpdater)
        {
            if (!historyColumns.TryGetValue(column.Id, out ColumnGetResponseDto? curColumn))
            {
                curColumn = column;
                historyColumns.Add(curColumn.Id, curColumn);
                curColumn.CreatedBy = columnCreator;
                curColumn.UpdatedBy = columnUpdater;
                curColumn.Cards = new();
            }

            if (card != null)
            {
                curColumn.Cards.Add(card);
                card.CreatedBy = cardCreator!;
                card.UpdatedBy = cardUpdater!;
            }

            return curColumn;
        }

        #endregion

        #endregion

        #region Card

        public async Task<CardGetResponseDto?> InsertCard(CardInsertDto insertedCard)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            int id = await connection.ExecuteScalarAsync<int>(@"insert into [Card] ([Title], [Description],
[ColumnId], [OrderIndex], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt]) 
values (@Title, @Description, @ColumnId, @OrderIndex, @CreatedBy, @CreatedAt, @CreatedBy, @CreatedAt);

select scope_identity();", 
                                                              insertedCard);
            return await GetCardByIdInternal(connection, id);
        }

        public async Task<List<CardGetResponseDto>> GetAllCards(int userId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            return await GetAllCardsInternal(connection, userId);
        }

        public async Task<CardGetResponseDto?> GetCardById(int cardId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            return await GetCardByIdInternal(connection, cardId);
        }

        public async Task<CardGetResponseDto?> UpdateCard(CardUpdateDto updatedCard)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            await connection.ExecuteAsync(@"update [Card] 
set [Card].[Title] = @Title, [Card].[Description] = @Description, [Card].[ColumnId] = @ColumnId, 
[Card].[OrderIndex] = @OrderIndex, [Card].[UpdatedBy] = @UpdatedBy, [Card].[UpdatedAt] = @UpdatedAt 
where [Card].[Id] = @Id", 
                                          updatedCard);
            
            return await GetCardByIdInternal(connection, updatedCard.Id);
        }

        public async Task<List<CardGetResponseDto>> DeleteCard(int userId, int cardId)
        {
            await using SqlConnection connection = new(_configuration.GetConnectionString(ConfigurationKeys.TaskManagerDbConnectionString));

            await connection.ExecuteAsync("delete from [Card] where [Card].[Id] = @CardId",
                                          new { CardId = cardId });

            return await GetAllCardsInternal(connection, userId);
        }

        #region CardInternal

        private async Task<List<CardGetResponseDto>> GetAllCardsInternal(SqlConnection connection, int userId)
        {
            IEnumerable<CardGetResponseDto> cards = await connection.QueryAsync<CardGetResponseDto,
                                                                                UserInfoDto,
                                                                                UserInfoDto,
                                                                                CardGetResponseDto>(
                @"select [Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], 
[Card].[OrderIndex], [Card].[CreatedAt], [Card].[UpdatedAt], [CardCreator].[Id], 
[CardCreator].[UserName], [CardCreator].[FirstName], [CardCreator].[LastName], [CardCreator].[Email], 
[CardUpdater].[Id], [CardUpdater].[UserName], [CardUpdater].[FirstName], [CardUpdater].[LastName], 
[CardUpdater].[Email] 
from [Card] 
left join [User] as [CardCreator] on [Card].[CreatedBy] = [CardCreator].[Id] 
left join [User] as [CardUpdater] on [Card].[UpdatedBy] = [CardUpdater].[Id] 
where [CardCreator].[Id] = @UserId",
                (card, cardCreator, cardUpdater) =>
                {
                    card.CreatedBy = cardCreator;
                    card.UpdatedBy = cardUpdater;
                    return card;
                },
                param: new { UserId = userId },
                splitOn: "Id, Id");
            
            return cards.Distinct().ToList();
        }

        private async Task<CardGetResponseDto?> GetCardByIdInternal(SqlConnection connection, int cardId)
        {
            IEnumerable<CardGetResponseDto> cards = await connection.QueryAsync<CardGetResponseDto,
                                                                                UserInfoDto,
                                                                                UserInfoDto,
                                                                                CardGetResponseDto>(
                @"select [Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], 
[Card].[OrderIndex], [Card].[CreatedAt], [Card].[UpdatedAt], [CardCreator].[Id], 
[CardCreator].[UserName], [CardCreator].[FirstName], [CardCreator].[LastName], [CardCreator].[Email], 
[CardUpdater].[Id], [CardUpdater].[UserName], [CardUpdater].[FirstName], [CardUpdater].[LastName], 
[CardUpdater].[Email] 
from [Card] 
left join [User] as [CardCreator] on [Card].[CreatedBy] = [CardCreator].[Id] 
left join [User] as [CardUpdater] on [Card].[UpdatedBy] = [CardUpdater].[Id] 
where [Card].[Id] = @CardId",
                (card, cardCreator, cardUpdater) =>
                {
                    card.CreatedBy = cardCreator;
                    card.UpdatedBy = cardUpdater;
                    return card;
                },
                param: new { CardId = cardId },
                splitOn: "Id, Id");
            
            var cardsList = cards.Distinct().ToList();
            return cardsList.Count == 0 
                ? null 
                : cardsList[0];
        }

        #endregion

        #endregion
    }
}