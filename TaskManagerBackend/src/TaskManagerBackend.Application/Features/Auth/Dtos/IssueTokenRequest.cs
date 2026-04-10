using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagerBackend.Application.Features.Auth.Dtos;

public class IssueTokenRequest
{
    [Required]
    [JsonPropertyName("grant_type")]
    public string GrantType { get; init; } = null!;
    
    [JsonPropertyName("username")]
    public string? Username { get; init; }
    
    [JsonPropertyName("password")]
    public string? Password { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}