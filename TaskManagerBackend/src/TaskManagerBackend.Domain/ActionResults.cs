namespace TaskManagerBackend.Domain;

/// <summary>
///     Provides values to describe typical results of actions.
/// </summary>
public enum ActionResults
{
    /// <summary>
    ///     Represents common successful result.
    /// </summary>
    Success,
    
    /// <summary>
    ///     Represents error on user side like invalid format of passed data.
    ///     Such errors must be checked on client side, but server side also checks them to prevent API misusage.
    /// </summary>
    UserError,
    
    /// <summary>
    ///     Represents error when user can not provide authentication or authorization data,
    ///     or invalid data was provided.
    /// </summary>
    Unauthorized,
    
    /// <summary>
    ///     Represents error when valid user has no access to resource or action.
    /// </summary>
    AccessDenied,
    
    /// <summary>
    ///     Represents error when trying request action on non-existing resource.
    /// </summary>
    ResourceNotFound,
    
    /// <summary>
    ///     Represents error during write-action, for example, due to multi-user conflicting actions
    ///     or when valid data was passed, but domain rules forbid action.
    /// </summary>
    DataConflict,
    
    // TODO: Reconsider usages
    /// <summary>
    ///     Represents abstract server error.
    /// </summary>
    /// <remarks>
    ///     Usually this should be an actual Exception.
    /// </remarks>
    ServerError
}