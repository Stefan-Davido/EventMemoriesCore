namespace DalEntities
{
    public class Info
    {
        public Guid Id { get; set; }
        public InfoLevel Level { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }

        public Event Event { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
