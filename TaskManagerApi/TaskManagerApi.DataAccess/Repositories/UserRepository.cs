using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Tuple<int, byte[], byte[]>?> GetUserPasswordData(string logInData)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            try
            {
                (int id, byte[] passwordHash, byte[] passwordSalt) = await connection.QueryFirstAsync<(int, byte[], byte[])>(
                    "select [Id], [PasswordHash], [PasswordSalt] from [User] where [UserName] = @LogInData or [Email] = @LogInData",
                    new { LogInData = logInData });
                return new (id, passwordHash, passwordSalt);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        async Task<bool> IUserRepository.CheckIfUserExistsByUserNameOrEmail(string userName, string email)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            DynamicParameters parameters = new();
            parameters.Add("@UserName", userName);
            parameters.Add("@Email", email);

            try
            {
                UserSignUpResponseDto user = await connection.QueryFirstAsync<UserSignUpResponseDto>(
                    "select [UserName], [Email] from [User] where [UserName] = @UserName or [Email] = @Email",
                    parameters);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        async Task IUserRepository.Insert(User user)
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync(
                "insert into [User] (UserName, Email, CreatedAt, UpdatedAt, PasswordHash, PasswordSalt) values " +
                "(@UserName, @Email, @CreatedAt, @UpdatedAt, @PasswordHash, @PasswordSalt)", user);
            
            return;
        }
    }
}