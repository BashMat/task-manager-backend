using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;

namespace TaskManagerApi.Services
{
	public class AuthService : IAuthService
	{
		private readonly IConfiguration _configuration;
		private readonly DataContext _dataContext;

		public AuthService(IConfiguration configuration, DataContext dataContext)
		{
			_configuration = configuration;
			_dataContext = dataContext;
		}

		public async Task<ServiceResponse<User>> SignUp(UserDto user)
		{
			var response = new ServiceResponse<User>();
			var users = await _dataContext.Users.ToListAsync();
			if (users.FirstOrDefault(u => u.UserName == user.UserName) != null)
			{
				response.Data = null;
				response.Success = false;
				response.Message = "Username already exists";
			}
			else
			{
				var newUser = new User();
				newUser.UserName = user.UserName;
				newUser.Email = user.Email;
				byte[] passwordHash, passwordSalt;
				CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);
				newUser.PasswordHash = passwordHash;
				newUser.PasswordSalt = passwordSalt;
				await _dataContext.Users.AddAsync(newUser);
				await _dataContext.SaveChangesAsync();

				response.Data = newUser;
			}
			return response;
		}

		public async Task<ServiceResponse<string>> Login(UserDto user)
		{
			var response = new ServiceResponse<string>();

			var users = await _dataContext.Users.ToListAsync();
			var userInList = users.FirstOrDefault(u => u.UserName == user.UserName);
			if (userInList != null && VerifyPasswordHash(user.Password, userInList.PasswordHash, userInList.PasswordSalt))
			{
				response.Data = CreateToken(userInList);
			}
			else
			{
				response.Data = null;
				response.Success = false;
				response.Message = "Incorrect username/password pair";
			}
			return response;
		}

		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				return passwordHash.SequenceEqual(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
			}
		}

		public string CreateToken(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName)
			};

			//var expire = DateTime.Now.AddHours(4);
			var expire = DateTime.Now.AddSeconds(30);
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token").Value));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
			var token = new JwtSecurityToken(null, null, claims, null, expire, creds);
			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}
	}
}
