namespace DalEntities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public Guid OwnerId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? EventDateEnd { get; set; }
        public string? Description { get; set; }
        public TenantSubscription Subscription { get; set; } = TenantSubscription.S;

        public ApplicationUser Owner { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Info> Infos { get; set; } = new List<Info>();
        public ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();
    }
}
