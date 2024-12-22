using TaskManagerBackend.Dto.User;
using Models = TaskManagerBackend.Domain.Models;

namespace TaskManagerBackend.DataAccess.Repositories.User;

public interface IUserRepository
{
    public Task<UserCredentialsData?> GetUserPasswordData(string logInData);
    public Task<bool> CheckIfUserExistsById(int id);
    public Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email);
    public Task Insert(Models.User user);
}