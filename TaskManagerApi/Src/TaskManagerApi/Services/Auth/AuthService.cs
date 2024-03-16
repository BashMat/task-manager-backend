using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApi.DataAccess.Repositories.User;
using TaskManagerApi.Common;

namespace TaskManagerApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        private const string UserAlreadyExistsMessage = "Username and/or Email already exists";
        private const string UserDoesNotExistMessage = "This user does not exist";
        private const string IncorrectCredentialsMessage = "Incorrect username/password pair";

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData)
        {
            ServiceResponse<UserSignUpResponseDto> response = new ();

            if (await _userRepository.CheckIfUserExistsByUserNameOrEmail(requestData.UserName, requestData.Email))
            {
                response.Data = null;
                response.Success = false;
                response.Message = UserAlreadyExistsMessage;
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

        public async Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData)
        {
            ServiceResponse<string> response = new();

            Tuple<int, byte[], byte[]>? passwordData = await _userRepository.GetUserPasswordData(requestData.LogInData);

            if (passwordData == null)
            {
                response.Data = null;
                response.Success = false;
                response.Message = UserDoesNotExistMessage;
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
            response.Message = IncorrectCredentialsMessage;

            return response;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            return passwordHash.SequenceEqual(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private string CreateToken(int userId)
        {
            var claims = new List<Claim>
            {
                new ("sub", userId.ToString())
            };

            //var expire = DateTime.Now.AddHours(4);
            var expire = DateTime.Now.AddMinutes(30);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[ConfigurationKeys.Token]!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(null, null, claims, null, expire, signingCredentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
