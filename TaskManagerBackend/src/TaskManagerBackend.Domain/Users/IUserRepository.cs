#region Usings

using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Domain.Users;

public interface IUserRepository
{
    public Task<UserPasswordData?> GetUserPasswordData(string logInData);
    public Task<bool> CheckIfUserExistsById(int id);
    public Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email);
    public Task InsertUser(NewUser newUser);
}