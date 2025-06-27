#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Domain.Validation;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

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

    public async Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest requestData)
    {
        // TODO: Think about usage. When validation is added via attributes, there is already attribute for email address. Perhaps should modify.
        if (!_emailValidator.Validate(requestData.Email))
        {
            _logger.LogTrace("Invalid email address format");
            
            return new ServiceResponse<UserSignUpResponse>(actionResult: ActionResults.UserError,
                                                           message: InvalidEmailAddressMessage);
        }

        if (await _userRepository.CheckIfUserExistsByUserNameOrEmail(requestData.UserName, requestData.Email))
        {
            _logger.LogTrace("User already exists");
            
            return new ServiceResponse<UserSignUpResponse>(actionResult: ActionResults.DataConflict,
                                                           message: UserAlreadyExistsMessage);
        }

        _logger.LogTrace("Start user registration");

        (byte[] passwordHash, byte[] passwordSalt) =
            _cryptographyService.CreatePasswordHashAndSalt(requestData.Password);
            
        NewUser newUser = new(_dateTimeService, requestData.UserName, requestData.Email, passwordHash, passwordSalt);
        await _userRepository.InsertUser(newUser);

        UserSignUpResponse response = new()
                                      {
                                          UserName = requestData.UserName,
                                          Email = requestData.Email
                                      };

        _logger.LogTrace("Finish user registration");

        return response;
    }

    public async Task<ServiceResponse<string>> LogIn(UserLogInRequest requestData)
    {
        UserPasswordData? data = await _userRepository.GetUserPasswordData(requestData.LogInData);

        if (data is null)
        {
            _logger.LogTrace("User does not exist");

            return new ServiceResponse<string>(actionResult: ActionResults.Unauthorized,
                                               message: IncorrectCredentialsMessage);
        }

        if (_cryptographyService.VerifyPasswordHash(requestData.Password, data.PasswordHash, data.PasswordSalt))
        {
            _logger.LogTrace("Password hash was verified");
            
            return _cryptographyService.CreateToken(data.Id);
        }

        _logger.LogTrace("Password hash was not verified");

        return new ServiceResponse<string>(actionResult: ActionResults.Unauthorized,
                                           message: IncorrectCredentialsMessage);
    }
}