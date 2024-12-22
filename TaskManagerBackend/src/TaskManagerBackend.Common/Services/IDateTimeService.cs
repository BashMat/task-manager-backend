namespace TaskManagerBackend.Common.Services;

/// <summary>
///     Represents service object for date and time related functionality.
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    ///     Gets <see cref="DateTime"/> instance set to current date and time for UTC.
    /// </summary>
    DateTime UtcNow { get; }
    
    /// <summary>
    ///     Gets <see cref="DateOnly"/> instance set to current date for UTC.
    /// </summary>
    DateOnly UtcDate { get; }
    
    /// <summary>
    ///     Gets <see cref="TimeOnly"/> instance set to current time for UTC.
    /// </summary>
    TimeOnly UtcTime { get; }
}