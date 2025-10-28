namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class Event
{
    public Guid Id { get; set; }
    
    public int EntityType { get; set; }
    
    public int EntityId { get; set; }
    
    public int EntityVersion { get; set; }
    
    public string Data { get; set; } = null!;
    
    public int DispatchedByUserId { get; set; }
    
    public DateTime DispatchedAt { get; set; }
    
    public Guid CorrelationId { get; set; } 
    
    public virtual User User { get; set; } = null!;
}