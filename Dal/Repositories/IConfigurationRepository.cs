using DalEntities;

namespace Dal.Repositories
{
    public interface IConfigurationRepository : IRepository<Configuration>
    {
        Task<IEnumerable<Configuration>> GetConfigurationsByEventAsync(Guid eventId);
        Task<Configuration?> GetConfigurationByNameAsync(Guid eventId, string name);
    }
}
