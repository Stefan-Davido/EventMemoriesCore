using DalEntities;

namespace Dal.Repositories
{
    public interface IInfoRepository : IRepository<Info>
    {
        Task<IEnumerable<Info>> GetInfosByEventAsync(Guid eventId);
        Task<IEnumerable<Info>> GetInfosByUserAsync(Guid userId);
        Task<IEnumerable<Info>> GetInfosByLevelAsync(InfoLevel level);
    }
}
