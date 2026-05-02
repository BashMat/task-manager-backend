#region Usings

using System.Text.Json.Serialization;
using TaskManagerBackend.Domain.Shared.Workflow;

#endregion

namespace TaskManagerBackend.Application.Utility;

public class ServiceResponse<T>(T? data = default,
                                ActionResultType? actionResultType = null,
                                string? message = null)
{
    public static implicit operator ServiceResponse<T>(T? data)
    {
        return new ServiceResponse<T>(data);
    }
    
    public T? Data { get; init; } = data;
    public bool Success => Data is not null && ActionResultType == ActionResultType.Success;
    public string? Message { get; init; } = message;

    [JsonIgnore]
    public ActionResultType ActionResultType { get; init; } = actionResultType ?? ActionResultType.Success;

    [JsonIgnore]
    public int? HttpStatusCode => ActionResultType.ToStatusCodesOrNull();
}