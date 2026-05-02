namespace TaskManagerBackend.Domain.Shared.Workflow;

/// <summary>
///     Represents interface for all application exceptions.
///     Every custom exception has to implement it to indicate that it is a processed exception
///     and not a non-processed exception from bug or other case.
/// </summary>
public interface IApplicationException
{
    ActionResultType ActionResultType { get; }
    string ResponseMessage { get; }
}