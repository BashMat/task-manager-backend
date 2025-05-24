#region Usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace TaskManagerBackend.Common.Services;

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

    public string CreateToken(int userId)
    {
        List<Claim> claims = new()
                             {
                                 new Claim(Claims.Sub, userId.ToString())
                             };

        DateTime expirationDateTime = GetExpirationDateTime();
        SecurityKey key = GetSigningKey();
        SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new JwtSecurityToken(null,
                                                      null,
                                                      claims,
                                                      null,
                                                      expirationDateTime,
                                                      signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public SecurityKey GetSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokensConfiguration.CurrentValue.Secret));
    }

    private DateTime GetExpirationDateTime()
    {
        return _dateTimeService.UtcNow.AddMinutes(_tokensConfiguration.CurrentValue.AccessTokenLifeTimeInMinutesAsDouble);
    }
}