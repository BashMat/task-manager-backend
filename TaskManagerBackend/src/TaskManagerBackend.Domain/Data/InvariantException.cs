namespace TaskManagerBackend.Domain.Data;

public class InvariantException(ActionResults actionResult, string message) : Exception(message), IApplicationException
{
    public ActionResults ActionResult { get; } = actionResult;
    public string ResponseMessage => base.Message;
}