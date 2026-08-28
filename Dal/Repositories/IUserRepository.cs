using DalEntities;

namespace Dal.Repositories
{
    public interface IUserRepository : IRepository<ApplicationUser, Guid>
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserWithPostsAsync(Guid userId);
    }
}
