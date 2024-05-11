using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagerBackend.Common;

namespace TaskManagerBackend.Application.Services.Auth;

public class AuthProvider : IAuthProvider
{
    private readonly IConfiguration _configuration;
    
    public AuthProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public ValueTuple<byte[], byte[]> CreatePasswordHashAndSalt(string password)
    {
        using var hmac = new HMACSHA512();
        byte[] passwordSalt = hmac.Key;
        byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (passwordHash, passwordSalt);
    }
    
    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        return passwordHash.SequenceEqual(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    public string CreateToken(int userId)
    {
        var claims = new List<Claim>
        {
            new (Claims.Sub, userId.ToString())
        };
            
        var expire = DateTime.Now.AddMinutes(30);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[ConfigurationKeys.Token]!));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(null, null, claims, null, expire, signingCredentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}