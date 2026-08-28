using DalEntities;

namespace Dal.Repositories
{
    public interface ITenantRepository : IRepository<Tenant, int>
    {
        Task<IEnumerable<Tenant>> GetTenantsByOwnerAsync(Guid ownerId);
        Task<Tenant?> GetTenantWithEventsAsync(int tenantId);
    }
}
