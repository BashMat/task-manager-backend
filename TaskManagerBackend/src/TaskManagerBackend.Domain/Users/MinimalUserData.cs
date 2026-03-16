namespace TaskManagerBackend.Domain.Users;

public class MinimalUserData(int id,
                             Usernames usernames)
{
    public int Id { get; } = id;
    public Usernames Usernames { get; } = usernames;
}
