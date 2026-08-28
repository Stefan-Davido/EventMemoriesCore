using DalEntities;

namespace Dal.Repositories
{
    public interface IRepository<T, TKey> where T : class
    {
        Task<T?> GetByIdAsync(TKey id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(TKey id);
        Task<bool> SoftDeleteAsync(TKey id);
        Task SaveChangesAsync();
    }
}
