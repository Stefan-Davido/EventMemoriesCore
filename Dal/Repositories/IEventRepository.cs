using DalEntities;

namespace Dal.Repositories
{
    public interface IEventRepository : IRepository<Event, Guid>
    {
        Task<IEnumerable<Event>> GetEventsByTenantAsync(int tenantId);
        Task<IEnumerable<Event>> GetEventsByOwnerAsync(Guid ownerId);
        Task<Event?> GetEventWithPostsAsync(Guid eventId);
        Task<Event?> GetEventWithDetailsAsync(Guid eventId);
    }
}
