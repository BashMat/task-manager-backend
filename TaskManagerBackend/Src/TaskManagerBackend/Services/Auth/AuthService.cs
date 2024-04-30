﻿using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Domain.Models;
using TaskManagerBackend.Dto.User;

namespace TaskManagerBackend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthProvider _authProvider;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        private const string UserAlreadyExistsMessage = "Username and/or Email already exists";
        private const string UserDoesNotExistMessage = "This user does not exist";
        private const string IncorrectCredentialsMessage = "Incorrect username/password pair";

        public AuthService(IAuthProvider authProvider, 
                           IUserRepository userRepository, 
                           ILogger<AuthService> logger)
        {
            _authProvider = authProvider;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData)
        {
            ServiceResponse<UserSignUpResponseDto> response = new();

            if (await _userRepository.CheckIfUserExistsByUserNameOrEmail(requestData.UserName, requestData.Email))
            {
                _logger.LogTrace("User already exists");
                
                response.Data = null;
                response.Success = false;
                response.Message = UserAlreadyExistsMessage;
                return response;
            }

            _logger.LogTrace("Start user registration");
            
            (byte[] passwordHash, byte[] passwordSalt) =
                _authProvider.CreatePasswordHashAndSalt(requestData.Password);
            
            User newUser = new(requestData.UserName, requestData.Email, passwordHash, passwordSalt);
            await _userRepository.Insert(newUser);

            response.Data = new UserSignUpResponseDto 
            {
                UserName = requestData.UserName,
                Email = requestData.Email
            };
            
            _logger.LogTrace("Finish user registration");
            
            return response;
        }

        public async Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData)
        {
            ServiceResponse<string> response = new();

            Tuple<int, byte[], byte[]>? passwordData = await _userRepository.GetUserPasswordData(requestData.LogInData);

            if (passwordData == null)
            {
                _logger.LogTrace("User does not exist");
                
                response.Data = null;
                response.Success = false;
                response.Message = UserDoesNotExistMessage;
                return response;
            }

            (int userId, byte[] passwordHash, byte[] passwordSalt) = passwordData;
            if (_authProvider.VerifyPasswordHash(requestData.Password, passwordHash, passwordSalt))
            {
                _logger.LogTrace("Password hash was verified");
                
                response.Data = _authProvider.CreateToken(userId);
                return response;
            }
            
            _logger.LogTrace("Password hash was not verified");

            response.Data = null;
            response.Success = false;
            response.Message = IncorrectCredentialsMessage;

            return response;
        }
    }
}
