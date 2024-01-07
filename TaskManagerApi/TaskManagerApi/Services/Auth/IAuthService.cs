using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;

namespace TaskManagerApi.Services.Auth
{
	public interface IAuthService
	{
		public Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData);

		public Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData);

		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

		public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
	}
}
