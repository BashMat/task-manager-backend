namespace TaskManagerBackend.Application.Exceptions;

/// <summary>
///     Represents exception used when token in invalid due to various reasons.
/// </summary>
public class InvalidTokenException : Exception, IApplicationException
{
    public InvalidTokenException() {}
    
    public InvalidTokenException(string message) : base(message) {}
}