using DalEntities;

namespace Dal.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<IEnumerable<Post>> GetPostsByEventAsync(Guid eventId);
        Task<IEnumerable<Post>> GetPostsByUserAsync(Guid userId);
    }
}
