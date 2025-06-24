using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public static class TrackingLogQueryables
{
    public static IQueryable<TrackingLog> FilterByCreator(this IQueryable<TrackingLog> logs, int userId)
    {
        return logs.Where(log => log.CreatedBy == userId);
    }
    
    public static IQueryable<T> FilterById<T>(this IQueryable<T> query, int id) where T : IEntity
    {
        return query.Where(item => item.Id == id);
    }
}