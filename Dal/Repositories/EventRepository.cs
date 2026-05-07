using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(EventMemoriesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Event>> GetEventsByTenantAsync(Guid tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByOwnerAsync(Guid ownerId)
        {
            return await _dbSet
                .Where(e => e.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Event?> GetEventWithPostsAsync(Guid eventId)
        {
            return await _dbSet
                .Include(e => e.Posts)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<Event?> GetEventWithDetailsAsync(Guid eventId)
        {
            return await _dbSet
                .Include(e => e.Posts)
                .Include(e => e.Infos)
                .Include(e => e.Configurations)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }
    }
}
