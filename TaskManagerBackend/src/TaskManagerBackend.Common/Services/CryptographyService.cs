#region Usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace TaskManagerBackend.Common.Services;

/// <inheritdoc/>
public class CryptographyService : ICryptographyService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IConfiguration _configuration;
    
    public CryptographyService(IDateTimeService dateTimeService,
                               IConfiguration configuration)
    {
        _dateTimeService = dateTimeService;
        _configuration = configuration;
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
            
        DateTime expire = _dateTimeService.UtcNow.AddMinutes(30);
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[ConfigurationKeys.Token]!));
        SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new JwtSecurityToken(null, null, claims, null, expire, signingCredentials);
        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}