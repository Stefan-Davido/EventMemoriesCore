namespace DalEntities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public Guid OwnerId { get; set; }

        public ApplicationUser Owner { get; set; } = null!;
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
