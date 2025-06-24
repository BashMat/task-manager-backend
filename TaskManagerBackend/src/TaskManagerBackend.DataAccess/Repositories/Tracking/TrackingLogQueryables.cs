using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public static class TrackingLogQueryables
{
    public static IQueryable<T> FilterByCreator<T>(this IQueryable<T> query, int userId) where T : IAuditedEntity
    {
        return query.Where(item => item.CreatedBy == userId);
    }
    
    public static IQueryable<T> FilterById<T>(this IQueryable<T> query, int id) where T : IEntity
    {
        return query.Where(item => item.Id == id);
    }

    // TODO: Should change approach by removing deeply nested data from querying.
    public static IQueryable<TrackingLog> SelectEagerly(this IQueryable<TrackingLog> logs)
    {
        return logs.Include(log => log.CreatedByNavigation)
                   .Include(log => log.UpdatedByNavigation)
                   .Include(log => log.TrackingLogEntries)
                   .ThenInclude(entry => entry.CreatedByNavigation)
                   .Include(log => log.TrackingLogEntries)
                   .ThenInclude(entry => entry.UpdatedByNavigation)
                   .Include(log => log.TrackingLogEntries)
                   .ThenInclude(entry => entry.Status)
                   .Include(log => log.Statuses)
                   .ThenInclude(entry => entry.CreatedByNavigation)
                   .Include(log => log.Statuses)
                   .ThenInclude(entry => entry.UpdatedByNavigation);
    }
}