#region Usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Auth;

#endregion

namespace TaskManagerBackend.Application.Utility.Security;

/// <inheritdoc/>
public class CryptographyService(IDateTimeService dateTimeService,
                                 IOptionsMonitor<TokensConfiguration> tokensConfiguration,
                                 ILogger<CryptographyService> logger) : ICryptographyService
{
    public ValueTuple<byte[], byte[]> CreatePasswordHashAndSalt(string password)
    {
        using HMACSHA512 hmac = new();
        byte[] passwordSalt = hmac.Key;
        byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (passwordHash, passwordSalt);
    }
    
    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using HMACSHA512 hmac = new(passwordSalt);
        return passwordHash.SequenceEqual(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
    
    public string IssueAccessToken(int userId)
    {
        DateTime issuedAt = dateTimeService.UtcNow;
        DateTime expiresAt = GetAccessTokenExpirationDateTime();
        
        return IssueTokenAsString(userId, null, issuedAt, expiresAt);
    }
    
    public RefreshTokenData IssueRefreshToken(int userId)
    {
        Guid tokenId = Guid.NewGuid();
        DateTime issuedAt = dateTimeService.UtcNow;
        DateTime expiresAt = GetRefreshTokenExpirationDateTime();

        return new RefreshTokenData(userId,
                                    tokenId,
                                    IssueTokenAsString(userId, tokenId, issuedAt, expiresAt),
                                    issuedAt,
                                    expiresAt);
    }
    
    // TODO: Currently in cases of high throughput identical tokens are issued for the same input data.
    //  Not a problem for a time, but has to be examined. Add some external randomized parameter.
    private string IssueTokenAsString(int userId, 
                                      Guid? tokenId, 
                                      DateTime issuedAt, 
                                      DateTime expiresAt)
    {
        List<Claim> claims =
        [
            new(Claims.Sub, userId.ToString(), ClaimValueTypes.Integer),
            new(Claims.IssuedAt,
                ((DateTimeOffset) issuedAt).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer)
        ];

        string? tokenIdAsStringOrNull = tokenId?.ToString();
        
        if (tokenIdAsStringOrNull is not null)
        {
            claims.Add(new Claim(Claims.TokenId, tokenIdAsStringOrNull, ClaimValueTypes.String));
        }
        
        SecurityKey key = GetSigningKey();
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new(null,
                                     null,
                                     claims,
                                     null,
                                     expiresAt,
                                     signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private SecurityKey GetSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokensConfiguration.CurrentValue.Secret));
    }
    
    public TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = GetSigningKey(),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.Zero
               };
    }

    public int? GetUserIdOrNull(string token)
    {
        try
        {
            JwtSecurityToken jwtToken = new(token);
            string? claimValue = jwtToken.Claims.GetClaimValueOrNull(Claims.Sub);

            return claimValue is null
                       ? null
                       : int.Parse(claimValue);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return null;
        }
    }
    
    public ServiceResponse<RefreshTokenData> ParseToken(string token)
    {
        try
        {
            // The first step of parsing: token must have JWT format, otherwise fail
            JwtSecurityToken jwtToken = new(token);
            
            // The second step of parsing: token integrity check
            new JwtSecurityTokenHandler().ValidateToken(token,
                                                        GetValidationParameters(),
                                                        out SecurityToken _);
            
            // The third step of parsing: getting actual token claims to build domain object
            int? userId = jwtToken.Claims.GetClaimValueOrNullAsInt(Claims.Sub);
            Guid? tokenId = jwtToken.Claims.GetClaimValueOrNullAsGuid(Claims.TokenId);
            DateTime? issuedAt = jwtToken.Claims.GetClaimValueOrNullAsDateTimeFromUnixSeconds(Claims.IssuedAt);
            DateTime? expiresAt = jwtToken.Claims.GetClaimValueOrNullAsDateTimeFromUnixSeconds(Claims.ExpiresAt);

            if (userId is null || tokenId is null || expiresAt is null || issuedAt is null)
            {
                return new ServiceResponse<RefreshTokenData>(actionResultType: ActionResultType.Unauthenticated,
                                                             message: AuthService.InvalidCredentialsMessage);
            }
            
            return new ServiceResponse<RefreshTokenData>(new RefreshTokenData(userId.Value,
                                                                              tokenId.Value,
                                                                              token,
                                                                              issuedAt.Value,
                                                                              expiresAt.Value));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return new ServiceResponse<RefreshTokenData>(actionResultType: ActionResultType.Unauthenticated,
                                                         message: AuthService.InvalidCredentialsMessage);
        }
    }

    private DateTime GetAccessTokenExpirationDateTime()
    {
        return dateTimeService.UtcNow.AddMinutes(tokensConfiguration.CurrentValue.AccessTokenLifeTimeInMinutesAsDouble);
    }
    
    private DateTime GetRefreshTokenExpirationDateTime()
    {
        return dateTimeService.UtcNow.AddMinutes(tokensConfiguration.CurrentValue.RefreshTokenLifeTimeInMinutesAsDouble);
    }
}