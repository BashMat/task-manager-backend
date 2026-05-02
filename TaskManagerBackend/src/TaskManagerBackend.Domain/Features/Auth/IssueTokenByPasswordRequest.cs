namespace TaskManagerBackend.Domain.Features.Auth;

/// <summary>
///     Represents domain representation for Issuing Token request in case of using User Password.
/// </summary>
public class IssueTokenByPasswordRequest
{
    public IssueTokenByPasswordRequest(string username, string password)
    {
        Username = username.Trim();
        Password = password.Trim();
    }
    
    public string Username { get; }
    public string Password { get; }
}