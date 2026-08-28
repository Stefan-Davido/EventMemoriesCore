using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class TenantRepository : Repository<Tenant, int>, ITenantRepository
    {
        public TenantRepository(EventMemoriesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Tenant>> GetTenantsByOwnerAsync(Guid ownerId)
        {
            return await _dbSet
                .Where(t => t.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Tenant?> GetTenantWithEventsAsync(int tenantId)
        {
            return await _dbSet
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }
    }
}
