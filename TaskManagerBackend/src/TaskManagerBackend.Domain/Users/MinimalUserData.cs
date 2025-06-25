namespace TaskManagerBackend.Domain.Users;

public class MinimalUserData
{
    public MinimalUserData(int id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
    }
    
    public int Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
}
