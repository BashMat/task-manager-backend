namespace TaskManagerBackend.Domain.Users;

public interface IUserRepository
{
    Task CreateUser(NewUser newUser);
    Task<MinimalUserData?> GetMinimalUserData(int id);
    Task<UserPasswordData?> GetUserPasswordData(int userId);
    Task<UserPasswordData?> GetUserPasswordData(string logInData);
    
    Task<bool> CheckIfUserHasNonExpiredRefreshToken(int userId, 
                                                    string refreshToken);
    Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, 
                                                  string email);
    
    Task<bool> UpdatePasswordData(UserPasswordData newPasswordData,
                                  DateTime updatedAt);

    #region Tokens

    Task CreateUserRefreshToken(int userId,
                                TokenData token,
                                string? invalidatedRefreshToken);
    Task DeleteUserRefreshTokens(int userId);

    #endregion
}