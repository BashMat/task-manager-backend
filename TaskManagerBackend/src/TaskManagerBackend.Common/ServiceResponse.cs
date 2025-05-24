namespace TaskManagerBackend.Common;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    
    public static implicit operator ServiceResponse<T>(T? data)
    {
        return new ServiceResponse<T>()
               {
                   Data = data
               };
    }
}