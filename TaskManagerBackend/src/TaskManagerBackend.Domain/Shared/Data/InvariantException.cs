using TaskManagerBackend.Domain.Shared.Workflow;

namespace TaskManagerBackend.Domain.Shared.Data;

public class InvariantException(ActionResultType actionResultType, string message) : Exception(message), IApplicationException
{
    public ActionResultType ActionResultType { get; } = actionResultType;
    public string ResponseMessage => base.Message;
}