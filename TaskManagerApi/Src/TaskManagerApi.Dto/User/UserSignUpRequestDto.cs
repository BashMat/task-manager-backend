namespace TaskManagerApi.Dto.User
{
	public class UserSignUpRequestDto
	{
		public string UserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
