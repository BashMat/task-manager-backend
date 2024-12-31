#region Usings

using TaskManagerBackend.Application.Services.Email;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Domain.Models;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthProvider _authProvider;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<AuthService> _logger;

    private const string UserAlreadyExistsMessage = "Username and/or Email already exists";
    private const string IncorrectCredentialsMessage = "Incorrect username/password pair";
    private const string InvalidEmailAddressMessage = "Email address has invalid format";

    public AuthService(IAuthProvider authProvider, 
                       IUserRepository userRepository,
                       IEmailService emailService,
                       IDateTimeService dateTimeService,
                       ILogger<AuthService> logger)
    {
        _authProvider = authProvider;
        _userRepository = userRepository;
        _emailService = emailService;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData)
    {
        ServiceResponse<UserSignUpResponseDto> response = new();

        if (!_emailService.ValidateEmailAddressFormat(requestData.Email))
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
            _authProvider.CreatePasswordHashAndSalt(requestData.Password);
            
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
            
        if (_authProvider.VerifyPasswordHash(requestData.Password, data.PasswordHash, data.PasswordSalt))
        {
            _logger.LogTrace("Password hash was verified");
                
            response.Data = _authProvider.CreateToken(data.Id);
            return response;
        }
            
        _logger.LogTrace("Password hash was not verified");

        response.Data = null;
        response.Success = false;
        response.Message = IncorrectCredentialsMessage;

        return response;
    }
}