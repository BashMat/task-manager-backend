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
    public async Task<MinimalUserData> CreateUser(NewUser newUser)
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
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Finishing inserting user data");

        return new MinimalUserData(user.Id,
                                   user.UserName,
                                   user.Email);
    }

    public async Task<MinimalUserData?> GetMinimalUserData(int id)
    {
        return await dbContext.Users.Where(u => u.Id == id)
                              .Select(u => new MinimalUserData(u.Id,
                                                               u.UserName,
                                                               u.Email))
                              .FirstOrDefaultAsync();
    }
    
    public async Task<UserPasswordData?> GetUserPasswordData(int userId)
    {
        logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await dbContext.Users.Where(u => u.Id == userId)
                                                .Select(u => new UserPasswordData(u.Id, u.PasswordHash, u.PasswordSalt))
                                                .FirstOrDefaultAsync();

        logger.LogInformation("Finishing getting user password data");
        
        return data;
    }
    
    public async Task<UserPasswordData?> GetUserPasswordData(string username)
    {
        logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await dbContext.Users.Where(u => u.UserName == username ||
                                                                  u.Email == username)
                                                .Select(u => new UserPasswordData(u.Id,
                                                                                  u.PasswordHash,
                                                                                  u.PasswordSalt))
                                                .FirstOrDefaultAsync();

        logger.LogInformation("Finishing getting user password data");
        
        return data;
    }

    public async Task<bool> CheckIfUserHasNonExpiredRefreshToken(int userId, Guid refreshTokenId)
    {
        return await dbContext.RefreshTokens.AnyAsync(rt => rt.Id == refreshTokenId &&
                                                            rt.UserId == userId &&
                                                            dateTimeService.UtcNow < rt.ExpiresAt);
    }

    public async Task<bool> CheckIfUserExistsById(int id)
    {
        logger.LogInformation("Starting checking if user exists by ID");
        
        bool result = await dbContext.Users.AnyAsync(u => u.Id == id);
        
        logger.LogInformation("Finishing checking if user exists by ID");

        return result;
    }

    public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
    {
        logger.LogInformation("Starting checking if user exists by user name or email");

        bool result = await dbContext.Users.AnyAsync(u => u.UserName == userName ||
                                                          u.Email == email);

        logger.LogInformation("Finishing checking if user exists by user name or email");
            
        return result;
    }
    
    public async Task<bool> UpdatePasswordData(UserPasswordData newPasswordData,
                                               DateTime updatedAt)
    {
        Database.Models.User? dbUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == newPasswordData.UserId);

        if (dbUser is null)
        {
            return false;
        }
        
        dbUser.PasswordHash = newPasswordData.PasswordHash;
        dbUser.PasswordSalt = newPasswordData.PasswordSalt;
        dbUser.UpdatedAt = updatedAt;
        
        await dbContext.SaveChangesAsync();

        return true;
    }

    #region Tokens

    public async Task CreateUserRefreshToken(int userId,
                                             RefreshTokenData refreshTokenData,
                                             Guid? invalidatedRefreshTokenId)
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

        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteUserRefreshTokens(int userId)
    {
        IQueryable<RefreshToken> previousUserTokens = 
            dbContext.RefreshTokens.Where(rt => rt.UserId == userId);
        dbContext.RefreshTokens.RemoveRange(previousUserTokens);
        
        await dbContext.SaveChangesAsync();
    }

    #endregion
}