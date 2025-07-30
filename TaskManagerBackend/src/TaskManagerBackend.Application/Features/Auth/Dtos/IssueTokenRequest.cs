using System.Text.Json.Serialization;

namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class IssueTokenRequest
{
    [JsonPropertyName("grant_type")]
    public required string GrantType { get; init; }
    
    [JsonPropertyName("username")]
    public string? UserName { get; init; }
    
    [JsonPropertyName("password")]
    public string? Password { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}