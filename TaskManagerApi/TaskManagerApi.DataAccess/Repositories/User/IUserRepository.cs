using Models = TaskManagerApi.Domain.Models;

namespace TaskManagerApi.DataAccess.Repositories.User
{
    public interface IUserRepository
    {
        public Task<Tuple<int, byte[], byte[]>?> GetUserPasswordData(string logInData);
        public Task<bool> CheckIfUserExistsById(int id);
        public Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email);
        public Task Insert(Models.User user);
    }
}
