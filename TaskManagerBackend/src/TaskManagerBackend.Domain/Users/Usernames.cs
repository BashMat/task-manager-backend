using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Users;

public record Usernames(StringAttribute AccountName, 
                        StringAttribute Email);