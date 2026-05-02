namespace TaskManagerBackend.Domain.Shared.Workflow;

/// <summary>
///     Provides type to describe result of action.
/// </summary>
public record ActionResultType
{
    private ActionResultType(string name)
    {
        Name = name;
    }
    
    public string Name { get; init; }
    public bool IsSuccess => this == Success;
    public bool IsError => !IsSuccess;

    /// <summary>
    ///     Represents common successful result.
    /// </summary>
    public static ActionResultType Success { get; } = new(nameof(Success));
    
    /// <summary>
    ///     Represents error on user side like invalid format of passed data.
    ///     Such errors must be checked on client side, but server side also checks them to prevent API misusage.
    /// </summary>
    public static ActionResultType UserError { get; } = new(nameof(UserError));
    
    /// <summary>
    ///     Represents error when user can not provide authentication data
    ///     or invalid data was provided.
    /// </summary>
    public static ActionResultType Unauthenticated { get; } = new(nameof(Unauthenticated));
    
    /// <summary>
    ///     Represents error when valid user has no access to resource or action.
    /// </summary>
    public static ActionResultType Unauthorized { get; } = new(nameof(Unauthorized));
    
    /// <summary>
    ///     Represents error when trying request action on non-existing resource.
    /// </summary>
    public static ActionResultType ResourceNotFound { get; } = new(nameof(ResourceNotFound));
    
    /// <summary>
    ///     Represents error during write-action, for example, due to multi-user conflicting actions
    ///     or when valid data was passed, but domain rules forbid action.
    /// </summary>
    public static ActionResultType DataConflict { get; } = new(nameof(DataConflict));
    
    /// <summary>
    ///     Represents error occuring during requesting not developed functionality.
    ///     MUST be used only during development and unavailable to common user.
    /// </summary>
    public static ActionResultType NotImplemented { get; } = new(nameof(NotImplemented));
    
    // TODO: Reconsider usages
    /// <summary>
    ///     Represents abstract server error.
    /// </summary>
    /// <remarks>
    ///     Usually this should be an actual Exception.
    /// </remarks>
    public static ActionResultType ServerError { get; } = new(nameof(ServerError));
}