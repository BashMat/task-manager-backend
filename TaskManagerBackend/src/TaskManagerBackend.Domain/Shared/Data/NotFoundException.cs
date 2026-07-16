using TaskManagerBackend.Domain.Shared.Workflow;

namespace TaskManagerBackend.Domain.Shared.Data;

/// <summary>
///     Represents common <see cref="IApplicationException"/> used in cases when action was to be performed on specific
///     resource, entity, etc., but it was not found or does not exist.
/// </summary>
public class NotFoundException : Exception, IApplicationException
{
    public ActionResultType ActionResultType => ActionResultType.ResourceNotFound;
    public string ResponseMessage => base.Message;

    public NotFoundException() : base(MessageResources.ResourceDoesNotExist) { }
    
    public NotFoundException(string message) : base(message) { }

    public static void ThrowIfNull(object? value)
    {
        if (value is null)
        {
            throw new NotFoundException();
        }
    }
}