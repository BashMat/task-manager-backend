namespace TaskManagerBackend.Application.Exceptions;

/// <summary>
///     Represents exception used when access is denied due to various reasons.
/// </summary>
public class AccessDeniedException : Exception, IApplicationException
{
    public AccessDeniedException() {}
    
    public AccessDeniedException(string message) : base(message) {}
}