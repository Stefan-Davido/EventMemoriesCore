using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class ConfigurationRepository : Repository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(EventMemoriesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Configuration>> GetConfigurationsByEventAsync(Guid eventId)
        {
            return await _dbSet
                .Where(c => c.EventId == eventId)
                .ToListAsync();
        }

        public async Task<Configuration?> GetConfigurationByNameAsync(Guid eventId, string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.EventId == eventId && c.Name == name);
        }
    }
}
