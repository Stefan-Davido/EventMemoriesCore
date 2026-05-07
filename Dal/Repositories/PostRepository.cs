using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(EventMemoriesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Post>> GetPostsByEventAsync(Guid eventId)
        {
            return await _dbSet
                .Where(p => p.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }
}
