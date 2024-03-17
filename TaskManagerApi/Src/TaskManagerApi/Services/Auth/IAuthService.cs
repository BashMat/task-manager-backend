using TaskManagerApi.Domain;
using TaskManagerApi.Dto.User;

namespace TaskManagerApi.Services.Auth
{
	public interface IAuthService
	{
		public Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto requestData);

		public Task<ServiceResponse<string>> LogIn(UserLogInRequestDto requestData);
	}
}
