namespace TaskManagerBackend.Domain.Users;

public interface IUserRepository
{
    public Task<UserPasswordData?> GetUserPasswordData(string logInData);
    Task<bool> CheckIfUserHasNonExpiredRefreshToken(int userId, 
                                                    string refreshToken);
    public Task<bool> CheckIfUserExistsById(int id);
    public Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email);
    public Task CreateUser(NewUser newUser);

    #region Tokens

    public Task CreateUserRefreshToken(int userId,
                                       TokenData token,
                                       string? invalidatedRefreshToken);
    Task DeleteUserRefreshTokens(int userId);

    #endregion
}