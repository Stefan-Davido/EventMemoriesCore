using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class InfoRepository : Repository<Info>, IInfoRepository
    {
        public InfoRepository(EventMemoriesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Info>> GetInfosByEventAsync(Guid eventId)
        {
            return await _dbSet
                .Where(i => i.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Info>> GetInfosByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Info>> GetInfosByLevelAsync(InfoLevel level)
        {
            return await _dbSet
                .Where(i => i.Level == level)
                .ToListAsync();
        }
    }
}
