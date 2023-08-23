using TaskManagerAPI.Dtos.Project;
using TaskManagerAPI.Dtos.User;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
	public interface IAuthService
	{
		public Task<ServiceResponse<User>> SignUp(UserDto user);

		public Task<ServiceResponse<string>> Login(UserDto user);

		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

		public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
	}
}
