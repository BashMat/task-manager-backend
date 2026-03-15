namespace TaskManagerBackend.Domain.Data;

public class InvariantException : Exception, IApplicationException
{
    public InvariantException(ActionResults actionResult, string message) : base(message)
    {
        ActionResult = actionResult;
    }

    public ActionResults ActionResult { get; }
    public string ResponseMessage => base.Message;
}