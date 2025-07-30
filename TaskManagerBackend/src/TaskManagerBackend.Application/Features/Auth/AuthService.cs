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

    [Obsolete($"{nameof(IssueToken)} method must be used to issue both Access and Refresh tokens")]
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
            
            return _cryptographyService.IssueAccessToken(data.Id);
        }

        _logger.LogTrace("Password hash was not verified");

        return new ServiceResponse<string>(actionResult: ActionResults.Unauthorized,
                                           message: IncorrectCredentialsMessage);
    }
    
    public async Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest requestData)
    {
        if (requestData is { GrantType: "password", UserName: not null, Password: not null })
        {
            UserPasswordData? data = await _userRepository.GetUserPasswordData(requestData.UserName);

            if (data is null)
            {
                _logger.LogTrace("User does not exist");

                return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                               message: IncorrectCredentialsMessage);
            }

            if (_cryptographyService.VerifyPasswordHash(requestData.Password, data.PasswordHash, data.PasswordSalt))
            {
                _logger.LogTrace("Password hash was verified");
            
                string accessToken = _cryptographyService.IssueAccessToken(data.Id);
                TokenData refreshToken = _cryptographyService.IssueRefreshToken(data.Id);
                await _userRepository.SetUserRefreshToken(data.Id, refreshToken);
                
                return new IssueTokenResponse()
                       {
                           AccessToken = accessToken,
                           ExpiresIn = (refreshToken.ExpiresAt - _dateTimeService.UtcNow).Seconds,
                           RefreshToken = refreshToken.Token,
                           TokenType = "Bearer"
                       };
            }

            _logger.LogTrace("Password hash was not verified");

            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: IncorrectCredentialsMessage);
        }
        else if (requestData.GrantType == "refresh_token")
        {
            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.NotImplemented);
        }
        
        return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.UserError);
    }
}