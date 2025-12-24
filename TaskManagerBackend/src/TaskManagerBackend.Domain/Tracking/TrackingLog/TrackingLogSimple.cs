using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogSimple
{
    public TrackingLogSimple(int id,
                             string title,
                             string? description,
                             int createdByUserId,
                             DateTime createdAt,
                             int updatedByUserId,
                             DateTime updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = updatedAt;
    }
    
    public int Id { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public int CreatedByUserId { get; }
    public DateTime CreatedAt { get; }
    public int UpdatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    private Queue<IEvent> UnprocessedEvents { get; } = new();
    
    public void Rename(string newTitle,
                       int userId, 
                       IDateTimeService dateTimeService)
    {
        var renamedFrom = Title;
        Title = newTitle;
        UpdatedByUserId = userId;
        UpdatedAt = dateTimeService.UtcNow;

        TrackingLogRenamedData data = new(renamedFrom, Title);
        
        UnprocessedEvents.Enqueue(new TrackingLogRenamed(Guid.NewGuid(),
                                                         Id,
                                                         data,
                                                         UpdatedByUserId,
                                                         UpdatedAt));
    }

    public void DispatchEvents(IEventStore eventStore,
                               int lastVersion)
    {
        while (UnprocessedEvents.TryDequeue(out var eventToDispatch))
        {
            lastVersion += 1;
            if (eventToDispatch is TrackingLogRenamed eventWithData)
            {
                eventStore.Append(eventWithData,
                                  lastVersion);
            }
        }
    }
}