using DalEntities;

namespace Dal.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetEventsByTenantAsync(Guid tenantId);
        Task<IEnumerable<Event>> GetEventsByOwnerAsync(Guid ownerId);
        Task<Event?> GetEventWithPostsAsync(Guid eventId);
        Task<Event?> GetEventWithDetailsAsync(Guid eventId);
    }
}
