#region

using TaskManagerBackend.Domain.Shared.Data;
using TaskManagerBackend.Domain.Shared.Entities;

#endregion

namespace TaskManagerBackend.Domain.Features.Tracking.TrackingLog;

public class TrackingLogEntity : IAuditedEntity
{
    public TrackingLogEntity(int id,
                             StringAttribute title,
                             StringAttribute? description,
                             int createdBy,
                             DateTime createdAt,
                             int updatedBy,
                             DateTime updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
    
    public int Id { get; }
    public StringAttribute Title { get; private set; }
    public StringAttribute? Description { get; private set; }
    public int CreatedBy { get; }
    public DateTime CreatedAt { get; }
    public int UpdatedBy { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void RenameToByUser(StringAttribute value,
                               int userId,
                               DateTime dateTime)
    {
        if (Title.Value != value.Value)
        {
            Title = value;
            UpdatedBy = userId;
            UpdatedAt = dateTime;
        }
    }
    
    public void EditDescriptionToByUser(StringAttribute? value,
                                        int userId,
                                        DateTime dateTime)
    {
        if (Description?.Value != value?.Value)
        {
            Description = value;
            UpdatedBy = userId;
            UpdatedAt = dateTime;
        }
    }
}