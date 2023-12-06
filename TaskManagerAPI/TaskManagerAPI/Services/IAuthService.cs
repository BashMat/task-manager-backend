using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;

namespace TaskManagerApi.Services
{
	public interface IAuthService
	{
		public Task<ServiceResponse<User>> SignUp(UserDto user);

		public Task<ServiceResponse<string>> Login(UserDto user);

		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

		public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
	}
}
