using Microsoft.AspNetCore.Identity;

namespace DalEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Tenant> OwnedTenants { get; set; } = new List<Tenant>();
        public ICollection<Event> OwnedEvents { get; set; } = new List<Event>();
        public ICollection<Info> Infos { get; set; } = new List<Info>();
    }
}
