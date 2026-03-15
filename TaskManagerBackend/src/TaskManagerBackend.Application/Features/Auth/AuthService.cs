#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Auth;
using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Domain.Validation;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

public class AuthService(ICryptographyService cryptographyService,
                         IUserRepository userRepository,
                         IEmailValidator emailValidator,
                         IDateTimeService dateTimeService,
                         ILogger<AuthService> logger) : IAuthService
{
    public const string UserAlreadyExistsMessage = "Username and/or Email already exists";
    public const string InvalidCredentialsMessage = "Invalid credentials";
    public const string InvalidEmailAddressMessage = "Email address has invalid format";
    
    public const string PasswordGrantType = "password";
    public const string RefreshTokenGrantType = "refresh_token";

    public async Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest request,
                                                                  CancellationToken cancellationToken)
    {
        Usernames newUsernames = new Usernames(StringAttribute.CreateRequired(request.UserName),
                                               StringAttribute.CreateRequired(request.Email));
        // TODO: Think about usage. When validation is added via attributes, there is already attribute for email address. Perhaps should modify.
        if (!emailValidator.Validate(newUsernames.Email.Value))
        {
            logger.LogTrace("Invalid email address format");
            
            return new ServiceResponse<UserSignUpResponse>(actionResult: ActionResults.UserError,
                                                           message: InvalidEmailAddressMessage);
        }

        if (await userRepository.CheckIfUserExistsByUsername(newUsernames, cancellationToken))
        {
            logger.LogTrace("User already exists");
            
            return new ServiceResponse<UserSignUpResponse>(actionResult: ActionResults.DataConflict,
                                                           message: UserAlreadyExistsMessage);
        }

        logger.LogTrace("Start user registration");

        (byte[] passwordHash, byte[] passwordSalt) =
            cryptographyService.CreatePasswordHashAndSalt(request.Password);
            
        NewUser newUser = new(dateTimeService,
                              newUsernames,
                              passwordHash,
                              passwordSalt);
        MinimalUserData user = await userRepository.CreateUser(newUser, cancellationToken);

        UserSignUpResponse response = new()
                                      {
                                          Id = user.Id,
                                          UserName = user.Usernames.AccountName.Value,
                                          Email = user.Usernames.Email.Value
                                      };

        logger.LogTrace("Finish user registration");

        return response;
    }
    
    public async Task<ServiceResponse<IssueTokenResponse>> IssueToken(IssueTokenRequest request,
                                                                      CancellationToken cancellationToken)
    {
        switch (request)
        {
            case { GrantType: PasswordGrantType, Username: not null, Password: not null }:
                return await IssueTokenByPassword(new IssueTokenByPasswordRequest(request.Username, request.Password), cancellationToken);
            case { GrantType: RefreshTokenGrantType, RefreshToken: not null }:
            {
                ServiceResponse<RefreshTokenData> refreshTokenResponse = 
                    cryptographyService.ParseToken(request.RefreshToken);

                return refreshTokenResponse.Success switch
                       {
                           true => await IssueTokenByRefreshToken(new IssueTokenByRefreshTokenRequest(refreshTokenResponse.Data!), cancellationToken),
                           false => new ServiceResponse<IssueTokenResponse>(actionResult: refreshTokenResponse.ActionResult,
                                                                            message: refreshTokenResponse.Message)
                       };
            }
            default:
                return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.UserError);
        }
    }
    
    private async Task<ServiceResponse<IssueTokenResponse>> IssueTokenByRefreshToken(IssueTokenByRefreshTokenRequest request,
                                                                                     CancellationToken cancellationToken)
    {
        if (await userRepository.CheckIfUserHasNonExpiredRefreshToken(request.RefreshToken.UserId, 
                                                                       request.RefreshToken.TokenId,
                                                                       cancellationToken))
        {
            return await IssueToken(request.RefreshToken.UserId,
                                    request.RefreshToken.TokenId,
                                    cancellationToken);
        }
            
        logger.LogTrace("Refresh token is invalid");

        return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                       message: InvalidCredentialsMessage);
    }

    private async Task<ServiceResponse<IssueTokenResponse>> IssueTokenByPassword(IssueTokenByPasswordRequest request,
                                                                                 CancellationToken cancellationToken)
    {
        UserPasswordData? data = await userRepository.GetUserPasswordData(request.Username, cancellationToken);

        if (data is null)
        {
            logger.LogTrace("User does not exist");

            return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                           message: InvalidCredentialsMessage);
        }

        if (cryptographyService.VerifyPasswordHash(request.Password, data.PasswordHash, data.PasswordSalt))
        {
            logger.LogTrace("Password hash was verified");
            
            return await IssueToken(data.UserId, null, cancellationToken);
        }

        logger.LogTrace("Password hash was not verified");

        return new ServiceResponse<IssueTokenResponse>(actionResult: ActionResults.Unauthorized,
                                                       message: InvalidCredentialsMessage);
    }
    
    private async Task<IssueTokenResponse> IssueToken(int userId,
                                                      Guid? invalidatedRefreshTokenId,
                                                      CancellationToken cancellationToken)
    {
        string accessToken = cryptographyService.IssueAccessToken(userId);
        RefreshTokenData refreshToken = cryptographyService.IssueRefreshToken(userId);
        await userRepository.CreateUserRefreshToken(userId, 
                                                     refreshToken, 
                                                     invalidatedRefreshTokenId,
                                                     cancellationToken);
                
        return new IssueTokenResponse
               {
                   AccessToken = accessToken,
                   ExpiresIn = (refreshToken.ExpiresAt - dateTimeService.UtcNow).Seconds,
                   RefreshToken = refreshToken.Token,
                   TokenType = "Bearer"
               };
    }
    
    public async Task RevokeTokens(int userId, CancellationToken cancellationToken)
    {
        await userRepository.DeleteUserRefreshTokens(userId, cancellationToken);
    }
}