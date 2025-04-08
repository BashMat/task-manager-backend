namespace TaskManagerBackend.Application.Exceptions;

// TODO: Implement common properties and methods for global exception handling: Status Codes, messages, etc.
/// <summary>
///     Represents interface for all application exceptions.
///     Every custom exception has to implement it to indicate that it is processed exception and not a non-processed
///     exception from bug or non-processed case.
/// </summary>
public interface IApplicationException
{
    
}