#region Usings

using TaskManagerBackend.Common;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Domain.Models;
using TaskManagerBackend.Domain.Validators;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ICryptographyService _cryptographyService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailValidator _emailValidator;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<AuthService> _logger;

    public const string UserAlreadyExistsMessage = "Username and/or Email already exists";
    public const string IncorrectCredentialsMessage = "Incorrect username/password pair";
    public const string InvalidEmailAddressMessage = "Email address has invalid format";

    public AuthService(ICryptographyService cryptographyService, 
                       IUserRepository userRepository,
                       IEmailValidator emailValidator,
                       IDateTimeService dateTimeService,
                       ILogger<AuthService> logger)
    {
        _cryptographyService = cryptographyService;
        _userRepository = userRepository;
        _emailValidator = emailValidator;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData)
    {
        ServiceResponse<UserSignUpResponseDto> response = new();

        if (!_emailValidator.ValidateEmailAddressFormat(requestData.Email))
        {
            _logger.LogTrace("Invalid email address format");
                
            response.Data = null;
            response.Success = false;
            response.Message = InvalidEmailAddressMessage;
            return response;
        }

        if (await _userRepository.CheckIfUserExistsByUserNameOrEmail(requestData.UserName, requestData.Email))
        {
            _logger.LogTrace("User already exists");

            response.Data = null;
            response.Success = false;
            response.Message = UserAlreadyExistsMessage;
            return response;
        }

        _logger.LogTrace("Start user registration");

        (byte[] passwordHash, byte[] passwordSalt) =
            _cryptographyService.CreatePasswordHashAndSalt(requestData.Password);
            
        User newUser = new(_dateTimeService, requestData.UserName, requestData.Email, passwordHash, passwordSalt);
        await _userRepository.Insert(newUser);

        response.Data = new UserSignUpResponseDto 
                        {
                            UserName = requestData.UserName,
                            Email = requestData.Email
                        };

        _logger.LogTrace("Finish user registration");

        return response;
    }

    public async Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData)
    {
        ServiceResponse<string> response = new();

        UserPasswordData? data = await _userRepository.GetUserPasswordData(requestData.LogInData);

        if (data is null)
        {
            _logger.LogTrace("User does not exist");

            response.Data = null;
            response.Success = false;
            response.Message = IncorrectCredentialsMessage;
            return response;
        }

        if (_cryptographyService.VerifyPasswordHash(requestData.Password, data.PasswordHash, data.PasswordSalt))
        {
            _logger.LogTrace("Password hash was verified");

            response.Data = _cryptographyService.CreateToken(data.Id);
            return response;
        }

        _logger.LogTrace("Password hash was not verified");

        response.Data = null;
        response.Success = false;
        response.Message = IncorrectCredentialsMessage;

        return response;
    }
}