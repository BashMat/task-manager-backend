#region Usings

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly TaskManagerDbContext _dbContext;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(TaskManagerDbContext dbContext,
                          IDateTimeService dateTimeService,
                          ILogger<UserRepository> logger)
    {
        _dbContext = dbContext;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }
    
    public async Task CreateUser(NewUser newUser)
    {
        _logger.LogInformation("Starting inserting user data");

        Database.Models.User user = new()
                                    {
                                        UserName = newUser.UserName,
                                        Email = newUser.Email,
                                        PasswordHash = newUser.PasswordHash,
                                        PasswordSalt = newUser.PasswordSalt,
                                        CreatedAt = newUser.CreatedAt,
                                        UpdatedAt = newUser.UpdatedAt
                                    };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Finishing inserting user data");
    }

    public async Task<MinimalUserData?> GetMinimalUserData(int id)
    {
        return await _dbContext.Users.Where(u => u.Id == id)
                                     .Select(u => new MinimalUserData(u.Id,
                                                                      u.UserName,
                                                                      u.Email))
                                     .FirstOrDefaultAsync();
    }
    
    public async Task<UserPasswordData?> GetUserPasswordData(int userId)
    {
        _logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await _dbContext.Users.Where(u => u.Id == userId)
                                                       .Select(u => new UserPasswordData(u.Id, u.PasswordHash, u.PasswordSalt))
                                                       .FirstOrDefaultAsync();

        _logger.LogInformation("Finishing getting user password data");
        
        return data;
    }
    
    public async Task<UserPasswordData?> GetUserPasswordData(string logInData)
    {
        _logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await _dbContext.Users.Where(u => u.UserName == logInData ||
                                                                   u.Email == logInData)
                                                       .Select(u => new UserPasswordData(u.Id, u.PasswordHash, u.PasswordSalt))
                                                       .FirstOrDefaultAsync();

        _logger.LogInformation("Finishing getting user password data");
        
        return data;
    }

    public async Task<bool> CheckIfUserHasNonExpiredRefreshToken(int userId, Guid refreshTokenId)
    {
        return await _dbContext.RefreshTokens.AnyAsync(rt => rt.Id == refreshTokenId && 
                                                             rt.UserId == userId && 
                                                             _dateTimeService.UtcNow < rt.ExpiresAt);
    }

    public async Task<bool> CheckIfUserExistsById(int id)
    {
        _logger.LogInformation("Starting checking if user exists by ID");
        
        bool result = await _dbContext.Users.AnyAsync(u => u.Id == id);
        
        _logger.LogInformation("Finishing checking if user exists by ID");

        return result;
    }

    public async Task<bool> CheckIfUserExistsByUserNameOrEmail(string userName, string email)
    {
        _logger.LogInformation("Starting checking if user exists by user name or email");

        bool result = await _dbContext.Users.AnyAsync(u => u.UserName == userName ||
                                                           u.Email == email);

        _logger.LogInformation("Finishing checking if user exists by user name or email");
            
        return result;
    }
    
    public async Task<bool> UpdatePasswordData(UserPasswordData newPasswordData,
                                               DateTime updatedAt)
    {
        Database.Models.User? dbUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == newPasswordData.UserId);

        if (dbUser is null)
        {
            return false;
        }
        
        dbUser.PasswordHash = newPasswordData.PasswordHash;
        dbUser.PasswordSalt = newPasswordData.PasswordSalt;
        dbUser.UpdatedAt = updatedAt;
        
        await _dbContext.SaveChangesAsync();

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
                _dbContext.RefreshTokens.Where(rt => rt.Id == invalidatedRefreshTokenId);
            _dbContext.RefreshTokens.RemoveRange(previousUserTokens);
        }
        
        _dbContext.RefreshTokens.Add(token);

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteUserRefreshTokens(int userId)
    {
        IQueryable<RefreshToken> previousUserTokens = 
            _dbContext.RefreshTokens.Where(rt => rt.UserId == userId);
        _dbContext.RefreshTokens.RemoveRange(previousUserTokens);
        
        await _dbContext.SaveChangesAsync();
    }

    #endregion
}