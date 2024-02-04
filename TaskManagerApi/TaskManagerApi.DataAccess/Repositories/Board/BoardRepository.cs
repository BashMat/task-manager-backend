using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagerApi.Domain.Dtos.Board;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.DataAccess.Repositories.Board
{
    public class BoardRepository : IBoardRepository
    {
        private readonly IConfiguration _configuration;

        public BoardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<BoardGetResponseDto> Insert(BoardInsertDto insertedBoard)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            int id = await connection.ExecuteScalarAsync<int>(
            "insert into [Board] (Title, Description, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) values " +
            "(@Title, @Description, @CreatedBy, @CreatedAt, @CreatedBy, @CreatedAt); " +
            "select scope_identity();", insertedBoard);

            IEnumerable<BoardGetResponseDto> boards = await connection.QueryAsync<BoardGetResponseDto,
                                                                                  UserInfoDto,
                                                                                  UserInfoDto,
                                                                                  BoardGetResponseDto>(
                "select [B].[Id], [B].[Title], [B].[Description], [B].[CreatedAt], [B].[UpdatedAt], " +
                "[Creator].[Id], [Creator].[UserName], [Creator].[FirstName], [Creator].[LastName], [Creator].[Email], " +
                "[Updater].[Id], [Updater].[UserName], [Updater].[FirstName], [Updater].[LastName], [Updater].[Email] " +
                "from [Board] as [B] " +
                "inner join [User] as [Creator] on [B].[CreatedBy] = [Creator].[Id] " +
                "inner join [User] as [Updater] on [B].[UpdatedBy] = [Updater].[Id] " +
                "where [B].[Id] = @BoardId",
                (board, userCreator, userUpdater) =>
                {
                    board.CreatedBy = userCreator;
                    board.UpdatedBy = userUpdater;
                    return board;
                },
                param: new { BoardId = id },
                splitOn: "Id, Id");
            return boards.First();
        }

        public async Task<List<BoardGetResponseDto>> GetAll(int userId)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            return await GetAllInternal(connection, userId);
        }

        public async Task<BoardGetResponseDto?> GetById(int boardId)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            Dictionary<int, BoardGetResponseDto> historyBoards = new();
            Dictionary<int, ColumnGetResponseDto> historyColumns = new();

            IEnumerable<BoardGetResponseDto> boards = await connection.QueryAsync<BoardGetResponseDto,
                                                                                  UserInfoDto,
                                                                                  UserInfoDto,
                                                                                  ColumnGetResponseDto,
                                                                                  CardGetResponseDto,
                                                                                  BoardGetResponseDto>(
                "select [B].[Id], [B].[Title], [B].[Description], [B].[CreatedAt], [B].[UpdatedAt], " +
                "[Creator].[Id], [Creator].[UserName], [Creator].[FirstName], [Creator].[LastName], [Creator].[Email], " +
                "[Updater].[Id], [Updater].[UserName], [Updater].[FirstName], [Updater].[LastName], [Updater].[Email], " +
                "[Col].[Id], [Col].[BoardId], [Col].[Title], [Col].[Description], [Col].[CreatedAt], [Col].[UpdatedAt], " +
                "[Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], [Card].[CreatedAt], [Card].[UpdatedAt] " +
                "from [Board] as [B] " +
                "inner join [User] as [Creator] on [B].[CreatedBy] = [Creator].[Id] " +
                "inner join [User] as [Updater] on [B].[UpdatedBy] = [Updater].[Id] " +
                "left join [Column] as [Col] on [B].[Id] = [Col].[BoardId] " +
                "left join [Card] on [Col].[Id] = [Card].[ColumnId] " +
                "where [B].[Id] = @BoardId",
                (board, userCreator, userUpdater, column, card) => 
                ToDto(historyBoards, historyColumns, board, userCreator, userUpdater, column, card),
                param: new { BoardId = boardId },
                splitOn: "Id, Id, Id, Id");
            return boards.Distinct().First();
        }

        #region Internal

        private async Task<List<BoardGetResponseDto>> GetAllInternal(SqlConnection connection, int userId)
        {
            Dictionary<int, BoardGetResponseDto> historyBoards = new();
            Dictionary<int, ColumnGetResponseDto> historyColumns = new();

            IEnumerable<BoardGetResponseDto> boards = await connection.QueryAsync<BoardGetResponseDto,
                                                              UserInfoDto,
                                                              UserInfoDto,
                                                              ColumnGetResponseDto,
                                                              CardGetResponseDto,
                                                              BoardGetResponseDto>(
                "select [B].[Id], [B].[Title], [B].[Description], [B].[CreatedAt], [B].[UpdatedAt], " +
                "[Creator].[Id], [Creator].[UserName], [Creator].[FirstName], [Creator].[LastName], [Creator].[Email], " +
                "[Updater].[Id], [Updater].[UserName], [Updater].[FirstName], [Updater].[LastName], [Updater].[Email], " +
                "[Col].[Id], [Col].[BoardId], [Col].[Title], [Col].[Description], [Col].[CreatedAt], [Col].[UpdatedAt], " +
                "[Card].[Id], [Card].[ColumnId], [Card].[Title], [Card].[Description], [Card].[CreatedAt], [Card].[UpdatedAt] " +
                "from [Board] as [B] " +
                "inner join [User] as [Creator] on [B].[CreatedBy] = [Creator].[Id] " +
                "inner join [User] as [Updater] on [B].[UpdatedBy] = [Updater].[Id] " +
                "left join [Column] as [Col] on [B].[Id] = [Col].[BoardId] " +
                "left join [Card] on [Col].[Id] = [Card].[ColumnId] " +
                "where [Creator].[Id] = @UserId",
                (board, userCreator, userUpdater, column, card) => 
                ToDto(historyBoards, historyColumns, board, userCreator, userUpdater, column, card),
                param: new { UserId = userId },
                splitOn: "Id, Id, Id, Id");
            return boards.Distinct().ToList();
        }

        private BoardGetResponseDto ToDto(Dictionary<int, BoardGetResponseDto> historyBoards,
                                   Dictionary<int, ColumnGetResponseDto> historyColumns,
                                   BoardGetResponseDto board,
                                   UserInfoDto userCreator,
                                   UserInfoDto userUpdater,
                                   ColumnGetResponseDto column,
                                   CardGetResponseDto card)
        {
            if (!historyBoards.TryGetValue(board.Id, out BoardGetResponseDto? curBoard))
            {
                curBoard = board;
                historyBoards.Add(curBoard.Id, curBoard);
                curBoard.CreatedBy = userCreator;
                curBoard.UpdatedBy = userUpdater;
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
                curColumn.Cards = new();
            }

            if (card != null)
            {
                curColumn.Cards.Add(card);
            }

            return curBoard;
        }

        #endregion

        public async Task<BoardGetResponseDto> Update(BoardUpdateDto updatedBoard)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync(
            "update [Board] " +
            "set [Board].[Title] = @Title, [Board].[Description] = @Description, " +
            "[Board].[UpdatedBy] = @UpdatedBy, [Board].[UpdatedAt] = @UpdatedAt " +
            "where [Board].[Id] = @Id", updatedBoard);

            IEnumerable<BoardGetResponseDto> boards = await connection.QueryAsync<BoardGetResponseDto,
                                                                                  UserInfoDto,
                                                                                  UserInfoDto,
                                                                                  BoardGetResponseDto>(
                "select [B].[Id], [B].[Title], [B].[Description], [B].[CreatedAt], [B].[UpdatedAt], " +
                "[Creator].[Id], [Creator].[UserName], [Creator].[FirstName], [Creator].[LastName], [Creator].[Email], " +
                "[Updater].[Id], [Updater].[UserName], [Updater].[FirstName], [Updater].[LastName], [Updater].[Email] " +
                "from [Board] as [B] " +
                "inner join [User] as [Creator] on [B].[CreatedBy] = [Creator].[Id] " +
                "inner join [User] as [Updater] on [B].[UpdatedBy] = [Updater].[Id] " +
                "where [B].[Id] = @BoardId",
                (board, userCreator, userUpdater) =>
                {
                    board.CreatedBy = userCreator;
                    board.UpdatedBy = userUpdater;
                    return board;
                },
                param: new { BoardId = updatedBoard.Id },
                splitOn: "Id");
            return boards.First();
        }

        public async Task<List<BoardGetResponseDto>> Delete(int userId, int boardId)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("delete from [Board] where [Board].[Id]=@BoardId",
                                          new { BoardId = boardId });

            return await GetAllInternal(connection, userId);
        }
    }
}