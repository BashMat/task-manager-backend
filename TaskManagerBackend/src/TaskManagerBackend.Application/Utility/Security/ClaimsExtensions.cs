#region Usings

using System.Security.Claims;

#endregion

namespace TaskManagerBackend.Application.Utility.Security;

public static class ClaimsExtensions
{
    public static string? GetClaimValueOrNull(this IEnumerable<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}