using TaskManagerBackend.Domain;

namespace TaskManagerBackend.Application.Exceptions;

/// <summary>
///     Represents exception used when token in invalid due to various reasons.
/// </summary>
public class InvalidTokenException() : Exception(InvalidTokenLogMessage), IApplicationException
{
    private const string InvalidTokenResponseMessage = "Invalid token";
    private const string InvalidTokenLogMessage = "Invalid token: user id was not provided or is invalid. Possibly it was requested in method that does not require token, which may mean error in endpoint configuration.";

    public ActionResults ActionResult => ActionResults.Unauthorized;

    public string ResponseMessage => InvalidTokenResponseMessage;
}