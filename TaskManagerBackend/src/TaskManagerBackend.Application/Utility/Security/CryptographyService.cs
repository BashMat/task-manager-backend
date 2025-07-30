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
    
    public CryptographyService(IDateTimeService dateTimeService,
                               IOptionsMonitor<TokensConfiguration> tokensConfiguration)
    {
        _dateTimeService = dateTimeService;
        _tokensConfiguration = tokensConfiguration;
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
                                 new Claim(Claims.Sub, userId.ToString())
                             };

        DateTime expiration = GetAccessTokenExpirationDateTime();
        
        return IssueToken(userId, claims, expiration).Token;
    }
    
    public TokenData IssueRefreshToken(int userId)
    {
        List<Claim> claims = new()
                             {
                                 new Claim(Claims.Sub, userId.ToString())
                             };

        DateTime expiration = GetRefreshTokenExpirationDateTime();

        return IssueToken(userId, claims, expiration);
    }

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

    private DateTime GetAccessTokenExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.AccessTokenLifeTimeInMinutesAsDouble);
    }
    
    private DateTime GetRefreshTokenExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.RefreshTokenLifeTimeInMinutesAsDouble);
    }
}