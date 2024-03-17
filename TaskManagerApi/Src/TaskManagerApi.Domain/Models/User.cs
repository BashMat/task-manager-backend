namespace TaskManagerApi.Domain.Models
{
	public class User
	{
		public string UserName { get; set; }
		public string Email { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		
        public User(string userName, string email, byte[] passwordHash, byte[] passwordSalt)
        {
	        DateTime createdAt = DateTime.UtcNow;

	        UserName = userName;
	        Email = email;
	        CreatedAt = createdAt;
	        UpdatedAt = createdAt;
	        PasswordHash = passwordHash;
	        PasswordSalt = passwordSalt;
        }
	}
}
