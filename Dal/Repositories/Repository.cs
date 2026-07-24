using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly EventMemoriesDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(EventMemoriesDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;


            if (entity is not IIsDeleted softDeleteEntity)
                return false; // or throw new InvalidOperationException($"{typeof(T).Name} does not support soft delete");

            softDeleteEntity.IsDeleted = true;
            _dbSet.Update(entity);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
