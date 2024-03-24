using TaskManagerBackend.Domain;
using TaskManagerBackend.Dto.User;

namespace TaskManagerBackend.Services.Auth
{
	public interface IAuthService
	{
		public Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData);

		public Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData);
	}
}
