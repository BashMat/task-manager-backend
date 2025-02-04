namespace TaskManagerBackend.Common.Services;

/// <inheritdoc/>
public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly UtcDate => DateOnly.FromDateTime(UtcNow);
    public TimeOnly UtcTime => TimeOnly.FromDateTime(UtcNow);
}