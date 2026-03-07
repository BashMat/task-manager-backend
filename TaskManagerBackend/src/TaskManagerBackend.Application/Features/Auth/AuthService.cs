#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Auth;
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

    public async Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest request)
    {
        // TODO: Think about usage. When validation is added via attributes, there is already attribute for email address. Perhaps should modify.
        if (!_emailValidator.Validate(request.Email))
        {
            _logger.LogTrace("Invalid email address format");
            
            return new ServiceResponse<UserSignUpResponse>(actionResult: ActionResults.UserError,
                                                           message: InvalidEmailAddressMessage);
        }

        if (await _userRepository.CheckIfUserExistsByUserNameOrEmail(request.UserName, request.Email))
        {
            _logger.LogTrace("User already exists");
            
            return new ServiceResponse<UserSignUpResponse>(actionResult: ActionResults.DataConflict,
                                                           message: UserAlreadyExistsMessage);
        }

        _logger.LogTrace("Start user registration");

        (byte[] passwordHash, byte[] passwordSalt) =
            _cryptographyService.CreatePasswordHashAndSalt(request.Password);
            
        NewUser newUser = new(_dateTimeService, request.UserName, request.Email, passwordHash, passwordSalt);
        await _userRepository.CreateUser(newUser);

        UserSignUpResponse response = new()
                                      {
                                          UserName = request.UserName,
                                          Email = request.Email
                                      };

        _logger.LogTrace("Finish user registration");

        return response;
    }
    
    public async Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest request)
    {
        switch (request)
        {
            case { GrantType: PasswordGrantType, Username: not null, Password: not null }:
                return await IssueTokenByPassword(new IssueTokenByPasswordRequest(request.Username, request.Password));
            case { GrantType: RefreshTokenGrantType, RefreshToken: not null }:
            {
                ServiceResponse<RefreshTokenData> refreshTokenResponse = 
                    _cryptographyService.ParseToken(request.RefreshToken);

                return refreshTokenResponse.Success switch
                       {
                           true => await IssueTokenByRefreshToken(new IssueTokenByRefreshTokenRequest(refreshTokenResponse.Data!)),
                           false => new ServiceResponse<IssueTokenResponse>(actionResult: refreshTokenResponse.ActionResult,
                                                                            message: refreshTokenResponse.Message)
                       };
            }
            default:
                return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.UserError);
        }
    }
    
    private async Task<ServiceResponse<IssueTokenResponse>> IssueTokenByRefreshToken(IssueTokenByRefreshTokenRequest request)
    {
        if (await _userRepository.CheckIfUserHasNonExpiredRefreshToken(request.RefreshToken.UserId, 
                                                                       request.RefreshToken.TokenId))
        {
            return await IssueToken(request.RefreshToken.UserId,
                                    request.RefreshToken.TokenId);
        }
            
        _logger.LogTrace("Refresh token is invalid");

        return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                       message: InvalidCredentialsMessage);
    }

    private async Task<ServiceResponse<IssueTokenResponse>> IssueTokenByPassword(IssueTokenByPasswordRequest request)
    {
        UserPasswordData? data = await _userRepository.GetUserPasswordData(request.Username);

        if (data is null)
        {
            _logger.LogTrace("User does not exist");

            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: InvalidCredentialsMessage);
        }

        if (_cryptographyService.VerifyPasswordHash(request.Password, data.PasswordHash, data.PasswordSalt))
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