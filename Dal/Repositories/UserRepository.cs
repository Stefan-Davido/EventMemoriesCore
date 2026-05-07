using DalEntities;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(EventMemoriesDbContext context) : base(context)
        {
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetUserWithPostsAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
