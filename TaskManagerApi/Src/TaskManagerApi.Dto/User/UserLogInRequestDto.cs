namespace TaskManagerApi.Dto.User
{
	public class UserLogInRequestDto
	{
        // UserName or Email
		public string LogInData { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
