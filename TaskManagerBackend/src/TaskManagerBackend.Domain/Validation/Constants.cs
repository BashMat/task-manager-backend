namespace TaskManagerBackend.Domain.Validation;

public static class Constants
{
    public const int MinIdentifier = 1;

    public const int MinPriority = 0;
    public const int MaxPriority = 100_000;

    public const double MinOrderIndex = 0.0d;
    public const double MaxOrderIndex = 100_000.0d;

    public const int MaxDefaultTextLength = 256;
    public const int MaxLongTextLength = 512;
}