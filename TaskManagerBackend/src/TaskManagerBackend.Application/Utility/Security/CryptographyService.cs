#region Usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Users;

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
        List<Claim> claims = new()
                             {
                                 new Claim(Claims.Sub, userId.ToString()),
                                 new Claim(Claims.IssuedAt, ((DateTimeOffset)_dateTimeService.UtcNow).ToUnixTimeSeconds().ToString())
                             };

        DateTime expiration = GetAccessTokenExpirationDateTime();
        
        return IssueToken(userId, claims, expiration).Token;
    }
    
    public TokenData IssueRefreshToken(int userId)
    {
        List<Claim> claims = new()
                             {
                                 new Claim(Claims.Sub, userId.ToString()),
                                 new Claim(Claims.IssuedAt, ((DateTimeOffset)_dateTimeService.UtcNow).ToUnixTimeSeconds().ToString())
                             };

        DateTime expiration = GetRefreshTokenExpirationDateTime();

        return IssueToken(userId, claims, expiration);
    }

    // TODO: Currently in cases of high throughput identical tokens are issued for the same input data.
    //  Not a problem for a time, but has to be examined. Add some external randomized parameter.
    private TokenData IssueToken(int userId, List<Claim> claims, DateTime expiration)
    {
        SecurityKey key = GetSigningKey();
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new(null,
                                     null,
                                     claims,
                                     null,
                                     expiration,
                                     signingCredentials);

        string issuedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenData()
               {
                   UserId = userId,
                   Token = issuedToken,
                   ExpiresAt = token.ValidTo,
                   IssuedAt = token.IssuedAt
               };
    }

    public SecurityKey GetSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokensConfiguration.CurrentValue.Secret));
    }

    public int? GetUserId(string token)
    {
        try
        {
            JwtSecurityToken jwtToken = new(token);
            string? claimValue = jwtToken.Claims.FirstOrDefault(c => c.Type == Claims.Sub)?.Value;

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

    private DateTime GetAccessTokenExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.AccessTokenLifeTimeInMinutesAsDouble);
    }
    
    private DateTime GetRefreshTokenExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.RefreshTokenLifeTimeInMinutesAsDouble);
    }
}