using DalEntities;

namespace Dal.Repositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserWithPostsAsync(Guid userId);
    }
}
