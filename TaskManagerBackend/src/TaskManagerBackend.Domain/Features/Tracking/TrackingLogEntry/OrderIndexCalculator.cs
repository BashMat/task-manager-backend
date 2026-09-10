using TaskManagerBackend.Domain.Shared.Validation;

namespace TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;

public static class OrderIndexCalculator
{
    public static decimal CalculateTargetOrderIndex(decimal? currentMinimum)
    {
        if (currentMinimum is null)
        {
            return (decimal)Constants.MinOrderIndex;
        }

        if (currentMinimum < (decimal)Constants.MinOrderIndex ||
            currentMinimum > (decimal)Constants.MaxOrderIndex)
        {
            throw new ArgumentException(null, nameof(currentMinimum));
        }

        return Math.Round(currentMinimum.Value / 2, 2);
    }
}