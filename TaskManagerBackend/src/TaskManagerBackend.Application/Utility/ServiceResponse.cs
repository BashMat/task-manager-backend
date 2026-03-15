#region Usings

using System.Text.Json.Serialization;
using TaskManagerBackend.Domain;

#endregion

namespace TaskManagerBackend.Application.Utility;

public class ServiceResponse<T>(T? data = default,
                                ActionResults actionResult = ActionResults.Success,
                                string? message = null)
{
    public static implicit operator ServiceResponse<T>(T? data)
    {
        return new ServiceResponse<T>(data);
    }
    
    public T? Data { get; init; } = data;
    public bool Success => Data is not null && ActionResult == ActionResults.Success;
    public string? Message { get; init; } = message;

    [JsonIgnore]
    public ActionResults ActionResult { get; init; } = actionResult;

    [JsonIgnore]
    public int? HttpStatusCode => ActionResult.ToStatusCodesOrNull();
}