using TaskManagerBackend.Domain.Features.Auth;

namespace TaskManagerBackend.Domain.Features.Users;

public interface IUserRepository
{
    Task<MinimalUserData> CreateUser(NewUser newUser, CancellationToken cancellationToken);
    Task<MinimalUserData?> GetMinimalUserData(int id, CancellationToken cancellationToken);
    Task<UserPasswordData?> GetUserPasswordData(int userId, CancellationToken cancellationToken);
    Task<UserPasswordData?> GetUserPasswordData(string username, CancellationToken cancellationToken);
    
    Task<bool> CheckIfUserHasNonExpiredRefreshToken(int userId,
                                                    Guid refreshTokenId,
                                                    CancellationToken cancellationToken);
    Task<bool> CheckIfUserExistsByUsername(Usernames usernames,
                                           CancellationToken cancellationToken);
    
    Task<bool> UpdatePasswordData(UserPasswordData newPasswordData,
                                  DateTime updatedAt,
                                  CancellationToken cancellationToken);

    #region Tokens

    Task CreateUserRefreshToken(int userId,
                                RefreshTokenData refreshTokenData,
                                Guid? invalidatedRefreshTokenId,
                                CancellationToken cancellationToken);
    Task DeleteUserRefreshTokens(int userId, CancellationToken cancellationToken);

    #endregion
}