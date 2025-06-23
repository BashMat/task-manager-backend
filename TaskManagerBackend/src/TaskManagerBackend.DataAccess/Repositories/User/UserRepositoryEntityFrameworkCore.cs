#region Usings

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.User;

public class UserRepositoryEntityFrameworkCore : IUserRepository
{
    private readonly TaskManagerDbContext _dbContext;
    private readonly ILogger<UserRepositoryEntityFrameworkCore> _logger;

    public UserRepositoryEntityFrameworkCore(TaskManagerDbContext dbContext,
                                             ILogger<UserRepositoryEntityFrameworkCore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UserPasswordData?> GetUserPasswordData(string logInData)
    {
        _logger.LogInformation("Starting getting user password data");

        UserPasswordData? data = await _dbContext.Users.Where(u => u.UserName == logInData ||
                                                                   u.Email == logInData)
                                                 .Select(u => new UserPasswordData()
                                                              {
                                                                  Id = u.Id,
                                                                  PasswordHash = u.PasswordHash,
                                                                  PasswordSalt = u.PasswordSalt
                                                              })
                                                 .FirstOrDefaultAsync();

        _logger.LogInformation("Finishing getting user password data");
        
        return data;
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

    public async Task InsertUser(NewUser newUser)
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
}