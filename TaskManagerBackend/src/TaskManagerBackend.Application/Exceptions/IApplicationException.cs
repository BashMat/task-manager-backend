namespace TaskManagerBackend.Application.Exceptions;

// TODO: Implement common properties and methods for global exception handling: Status Codes, messages, etc.
// Should separate inner application exception message (for logging) and client error message.
/// <summary>
///     Represents interface for all application exceptions.
///     Every custom exception has to implement it to indicate that it is a processed exception
///     and not a non-processed exception from bug or other case.
/// </summary>
public interface IApplicationException
{
    
}