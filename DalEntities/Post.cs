namespace DalEntities
{
    public class Post : IIsDeleted
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public bool IsDeleted { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}
