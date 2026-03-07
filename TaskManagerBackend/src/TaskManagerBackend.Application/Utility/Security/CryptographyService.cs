#region Usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Auth;

#endregion

namespace TaskManagerBackend.Application.Utility.Security;

/// <inheritdoc/>
public class CryptographyService : ICryptographyService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IOptionsMonitor<TokensConfiguration> _tokensConfiguration;
    private readonly ILogger<CryptographyService> _logger;

    public CryptographyService(IDateTimeService dateTimeService,
                               IOptionsMonitor<TokensConfiguration> tokensConfiguration,
                               ILogger<CryptographyService> logger)
    {
        _dateTimeService = dateTimeService;
        _tokensConfiguration = tokensConfiguration;
        _logger = logger;
    }
    
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
        DateTime issuedAt = _dateTimeService.UtcNow;
        DateTime expiresAt = GetAccessTokenExpirationDateTime();
        
        return IssueTokenAsString(userId, null, issuedAt, expiresAt);
    }
    
    public RefreshTokenData IssueRefreshToken(int userId)
    {
        Guid tokenId = Guid.NewGuid();
        DateTime issuedAt = _dateTimeService.UtcNow;
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
            new(Claims.Sub, userId.ToString()),
            new(Claims.IssuedAt,
                ((DateTimeOffset) issuedAt).ToUnixTimeSeconds().ToString())
        ];

        string? tokenIdAsStringOrNull = tokenId?.ToString();
        
        if (tokenIdAsStringOrNull is not null)
        {
            claims.Add(new Claim(Claims.TokenId, tokenIdAsStringOrNull));
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
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokensConfiguration.CurrentValue.Secret));
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
            _logger.LogError(e.Message);
            return null;
        }
    }
    
    public Guid? GetTokenIdOrNull(string token)
    {
        try
        {
            JwtSecurityToken jwtToken = new(token);
            string? claimValue = jwtToken.Claims.GetClaimValueOrNull(Claims.TokenId);

            return claimValue is null
                       ? null
                       : Guid.Parse(claimValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return null;
        }
    }
    
    public bool VerifyToken(string token)
    {
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token,
                                                        GetValidationParameters(),
                                                        out SecurityToken _);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }

    private DateTime GetAccessTokenExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.AccessTokenLifeTimeInMinutesAsDouble);
    }
    
    private DateTime GetRefreshTokenExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.RefreshTokenLifeTimeInMinutesAsDouble);
    }
}