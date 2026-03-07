#region Usings

using System.Security.Claims;

#endregion

namespace TaskManagerBackend.Application.Utility.Security;

public static class ClaimsExtensions
{
    public static string? GetClaimValueOrNull(this IEnumerable<Claim> claims, string claimType)
    {
        try
        {
            return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public static Guid? GetClaimValueOrNullAsGuid(this IEnumerable<Claim> claims, string claimType)
    {
        try
        {
            string? claimValue = claims.GetClaimValueOrNull(claimType);
            return claimValue is null
                       ? null
                       : Guid.Parse(claimValue);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public static DateTime? GetClaimValueOrNullAsDateTimeFromUnixSeconds(this IEnumerable<Claim> claims,
                                                                         string claimType)
    {
        try
        {
            string? claimValue = claims.GetClaimValueOrNull(claimType);
            DateTimeOffset? dateTimeOffset = claimValue is null
                                                 ? null
                                                 : DateTimeOffset.FromUnixTimeSeconds(long.Parse(claimValue));
            return dateTimeOffset?.UtcDateTime;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public static int? GetClaimValueOrNullAsInt(this IEnumerable<Claim> claims, string claimType)
    {
        try
        {
            string? claimValue = claims.GetClaimValueOrNull(claimType);
            return claimValue is null
                       ? null
                       : int.Parse(claimValue);
        }
        catch (Exception)
        {
            return null;
        }
    }
}