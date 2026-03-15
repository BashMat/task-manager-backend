using TaskManagerBackend.Domain;

namespace TaskManagerBackend.Application.Exceptions;

/// <summary>
///     Represents interface for all application exceptions.
///     Every custom exception has to implement it to indicate that it is a processed exception
///     and not a non-processed exception from bug or other case.
/// </summary>
public interface IApplicationException
{
    ActionResults ActionResult { get; }
    string ResponseMessage { get; }
}