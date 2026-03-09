#region Usings

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Auth;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.User;

public class UserRepository(TaskManagerDbContext dbContext,
                            IDateTimeService dateTimeService,
                            ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<MinimalUserData> CreateUser(NewUser newUser, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting inserting user data");

        Database.Models.User user = new()
                                    {
                                        UserName = newUser.UserName,
                                        Email = newUser.Email,
                                        PasswordHash = newUser.PasswordHash,
                                        PasswordSalt = newUser.PasswordSalt,
                                        CreatedAt = newUser.CreatedAt,
                                        UpdatedAt = newUser.UpdatedAt
                                    };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Finishing inserting user data");

        return new MinimalUserData(user.Id,
                                   user.UserName,
                                   user.Email);
    }

    public async Task<MinimalUserData?> GetMinimalUserData(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Users.Where(u => u.Id == id)
                              .Select(u => new MinimalUserData(u.Id,
                                                               u.UserName,
                                                               u.Email))
                              .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<UserPasswordData?> GetUserPasswordData(int userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await dbContext.Users.Where(u => u.Id == userId)
                                                .Select(u => new UserPasswordData(u.Id, u.PasswordHash, u.PasswordSalt))
                                                .FirstOrDefaultAsync(cancellationToken);

        logger.LogInformation("Finishing getting user password data");
        
        return data;
    }
    
    public async Task<UserPasswordData?> GetUserPasswordData(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await dbContext.Users.Where(u => u.UserName == username ||
                                                                  u.Email == username)
                                                .Select(u => new UserPasswordData(u.Id,
                                                                                  u.PasswordHash,
                                                                                  u.PasswordSalt))
                                                .FirstOrDefaultAsync(cancellationToken);

        logger.LogInformation("Finishing getting user password data");
        
        return data;
    }

    public async Task<bool> CheckIfUserHasNonExpiredRefreshToken(int userId, Guid refreshTokenId,
                                                                 CancellationToken cancellationToken)
    {
        return await dbContext.RefreshTokens.AnyAsync(rt => rt.Id == refreshTokenId &&
                                                            rt.UserId == userId &&
                                                            dateTimeService.UtcNow < rt.ExpiresAt,
                                                      cancellationToken);
    }

    public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email,
                                                               CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting checking if user exists by user name or email");

        bool result = await dbContext.Users.AnyAsync(u => u.UserName == userName ||
                                                          u.Email == email,
                                                     cancellationToken);

        logger.LogInformation("Finishing checking if user exists by user name or email");
            
        return result;
    }
    
    public async Task<bool> UpdatePasswordData(UserPasswordData newPasswordData,
                                               DateTime updatedAt,
                                               CancellationToken cancellationToken)
    {
        Database.Models.User? dbUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == newPasswordData.UserId,
                                                                                  cancellationToken);

        if (dbUser is null)
        {
            return false;
        }
        
        dbUser.PasswordHash = newPasswordData.PasswordHash;
        dbUser.PasswordSalt = newPasswordData.PasswordSalt;
        dbUser.UpdatedAt = updatedAt;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    #region Tokens

    public async Task CreateUserRefreshToken(int userId,
                                             RefreshTokenData refreshTokenData,
                                             Guid? invalidatedRefreshTokenId,
                                             CancellationToken cancellationToken)
    {
        RefreshToken token = new()
                             {
                                 Id = refreshTokenData.TokenId,
                                 UserId = userId,
                                 ExpiresAt = refreshTokenData.ExpiresAt,
                                 IssuedAt = refreshTokenData.IssuedAt
                             };
        
        if (invalidatedRefreshTokenId is not null)
        {
            IQueryable<RefreshToken> previousUserTokens = 
                dbContext.RefreshTokens.Where(rt => rt.Id == invalidatedRefreshTokenId);
            dbContext.RefreshTokens.RemoveRange(previousUserTokens);
        }
        
        dbContext.RefreshTokens.Add(token);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteUserRefreshTokens(int userId, CancellationToken cancellationToken)
    {
        IQueryable<RefreshToken> previousUserTokens = 
            dbContext.RefreshTokens.Where(rt => rt.UserId == userId);
        dbContext.RefreshTokens.RemoveRange(previousUserTokens);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}