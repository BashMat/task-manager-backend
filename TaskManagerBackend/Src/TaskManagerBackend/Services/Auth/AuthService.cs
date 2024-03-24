using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Models;
using TaskManagerBackend.Dto.User;

namespace TaskManagerBackend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthProvider _authProvider;
        private readonly IUserRepository _userRepository;

        private const string UserAlreadyExistsMessage = "Username and/or Email already exists";
        private const string UserDoesNotExistMessage = "This user does not exist";
        private const string IncorrectCredentialsMessage = "Incorrect username/password pair";

        public AuthService(IAuthProvider authProvider, IUserRepository userRepository)
        {
            _authProvider = authProvider;
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

            (byte[] passwordHash, byte[] passwordSalt) =
                _authProvider.CreatePasswordHashAndSalt(requestData.Password);
            
            User newUser = new(requestData.UserName, requestData.Email, passwordHash, passwordSalt);
            await _userRepository.Insert(newUser);

            response.Data = new UserSignUpResponseDto 
            {
                UserName = requestData.UserName,
                Email = requestData.Email
            };
            
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
            if (_authProvider.VerifyPasswordHash(requestData.Password, passwordHash, passwordSalt))
            {
                response.Data = _authProvider.CreateToken(userId);
                return response;
            }

            response.Data = null;
            response.Success = false;
            response.Message = IncorrectCredentialsMessage;

            return response;
        }
    }
}
