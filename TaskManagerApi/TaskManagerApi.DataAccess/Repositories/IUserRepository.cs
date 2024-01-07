using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.DataAccess.Repositories
{
    public interface IUserRepository
    {
        public Task<Tuple<int, byte[], byte[]>?> GetUserPasswordData(string logInData);
        public Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email);
        public Task Insert(User user);
    }
}
