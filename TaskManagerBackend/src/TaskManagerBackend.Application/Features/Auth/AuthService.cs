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
    public const string InvalidCredentialsMessage = "Invalid credentials";
    public const string InvalidEmailAddressMessage = "Email address has invalid format";
    
    public const string PasswordGrantType = "password";
    public const string RefreshTokenGrantType = "refresh_token";

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
        await _userRepository.CreateUser(newUser);

        UserSignUpResponse response = new()
                                      {
                                          UserName = requestData.UserName,
                                          Email = requestData.Email
                                      };

        _logger.LogTrace("Finish user registration");

        return response;
    }
    
    public async Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest requestData)
    {
        return requestData switch
               {
                   // TODO: Add explicit domain classes to enforce nullability rules
                   { GrantType: PasswordGrantType, Username: not null, Password: not null } => 
                       await IssueTokenByPassword(requestData),
                   { GrantType: RefreshTokenGrantType, RefreshToken: not null } => 
                       await IssueTokenByRefreshToken(requestData),
                   _ => new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.UserError)
               };
    }
    
    private async Task<ServiceResponse<IssueTokenResponse>> IssueTokenByRefreshToken(IssueTokenRequest requestData)
    {
        if (!_cryptographyService.VerifyToken(requestData.RefreshToken!))
        {
            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: InvalidCredentialsMessage);
        }
        
        int? userId = _cryptographyService.GetUserIdOrNull(requestData.RefreshToken!);

        if (userId is null)
        {
            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: InvalidCredentialsMessage);
        }

        Guid? tokenId = _cryptographyService.GetTokenIdOrNull(requestData.RefreshToken!);

        if (tokenId is null)
        {
            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: InvalidCredentialsMessage);
        }
        
        if (await _userRepository.CheckIfUserHasNonExpiredRefreshToken(userId.Value, 
                                                                       tokenId.Value))
        {
            return await IssueToken(userId.Value,
                                    tokenId.Value);
        }
            
        _logger.LogTrace("Refresh token is invalid");

        return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                       message: InvalidCredentialsMessage);
    }

    private async Task<ServiceResponse<IssueTokenResponse>> IssueTokenByPassword(IssueTokenRequest requestData)
    {
        UserPasswordData? data = await _userRepository.GetUserPasswordData(requestData.Username!);

        if (data is null)
        {
            _logger.LogTrace("User does not exist");

            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: InvalidCredentialsMessage);
        }

        if (_cryptographyService.VerifyPasswordHash(requestData.Password!, data.PasswordHash, data.PasswordSalt))
        {
            _logger.LogTrace("Password hash was verified");
            
            return await IssueToken(data.UserId);
        }

        _logger.LogTrace("Password hash was not verified");

        return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                       message: InvalidCredentialsMessage);
    }
    
    private async Task<IssueTokenResponse> IssueToken(int userId,
                                                      Guid? invalidatedRefreshTokenId = null)
    {
        string accessToken = _cryptographyService.IssueAccessToken(userId);
        RefreshTokenData refreshToken = _cryptographyService.IssueRefreshToken(userId);
        await _userRepository.CreateUserRefreshToken(userId, 
                                                     refreshToken, 
                                                     invalidatedRefreshTokenId);
                
        return new IssueTokenResponse
               {
                   AccessToken = accessToken,
                   ExpiresIn = (refreshToken.ExpiresAt - _dateTimeService.UtcNow).Seconds,
                   RefreshToken = refreshToken.Token,
                   TokenType = "Bearer"
               };
    }
    
    public async Task RevokeTokens(int userId)
    {
        await _userRepository.DeleteUserRefreshTokens(userId);
    }
}