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
using Dapper;
using Microsoft.Data.SqlClient;
using TaskManagerApi.DataAccess.Data;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.DataAccess.Repositories;
using Azure;

namespace TaskManagerApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        async Task<ServiceResponse<UserSignUpResponseDto>> IAuthService.SignUp(UserSignUpRequestDto requestData)
        {
            ServiceResponse<UserSignUpResponseDto> response = new ();

            if(await _userRepository.CheckIfUserExistsByUserNameOrEmail(requestData.UserName, requestData.Email))
            {
                response.Data = null;
                response.Success = false;
                response.Message = "Username and/or Email already exists";
                return response;
            }

            UserSignUpResponseDto responseData = new()
            {
                UserName = requestData.UserName,
                Email = requestData.Email
            };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(requestData.Password, out passwordHash, out passwordSalt);

            DateTime createdAt = DateTime.UtcNow;
            User newUser = new()
            {
                UserName = responseData.UserName,
                Email = responseData.Email,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _userRepository.Insert(newUser);

            response.Data = responseData;
            return response;
        }

        async Task<ServiceResponse<string>> IAuthService.LogIn(UserLogInRequestDto requestData)
        {
            ServiceResponse<string> response = new();

            Tuple<int, byte[], byte[]>? passwordData = await _userRepository.GetUserPasswordData(requestData.LogInData);

            if (passwordData == null)
            {
                response.Data = null;
                response.Success = false;
                response.Message = "This user does not exist";
                return response;
            }

            (int userId, byte[] passwordHash, byte[] passwordSalt) = passwordData;
            if (VerifyPasswordHash(requestData.Password, passwordHash, passwordSalt))
            {
                response.Data = CreateToken(userId);
                return response;
            }

            response.Data = null;
            response.Success = false;
            response.Message = "Incorrect username/password pair";

            return response;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                return passwordHash.SequenceEqual(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        public string CreateToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", userId.ToString())
            };

            //var expire = DateTime.Now.AddHours(4);
            var expire = DateTime.Now.AddMinutes(30);
            var key = new SymmetricSecurityKey(GetKey());
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(null, null, claims, null, expire, creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private byte[] GetKey()
        {
            try
            {
                return Encoding.UTF8.GetBytes(_configuration.GetSection("Token").Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Possibly token key was not specified");
            }
        }
    }
}
