using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Users;

public class MinimalUserData
{
    public MinimalUserData(int id,
                           Usernames usernames)
    {
        Id = id;
        Usernames = usernames;
    }
    
    public int Id { get; init; }
    public Usernames Usernames { get; init; }
}
