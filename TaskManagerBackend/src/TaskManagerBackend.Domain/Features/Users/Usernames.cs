using TaskManagerBackend.Domain.Shared.Data;

namespace TaskManagerBackend.Domain.Features.Users;

public record Usernames(StringAttribute AccountName, 
                        StringAttribute Email);