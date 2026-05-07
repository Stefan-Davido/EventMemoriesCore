using DalEntities;

namespace Dal.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<IEnumerable<Tenant>> GetTenantsByOwnerAsync(Guid ownerId);
        Task<Tenant?> GetTenantWithEventsAsync(Guid tenantId);
    }
}
