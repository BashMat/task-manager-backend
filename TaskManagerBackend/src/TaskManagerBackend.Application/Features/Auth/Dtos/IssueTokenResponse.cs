using System.Text.Json.Serialization;

namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class IssueTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}