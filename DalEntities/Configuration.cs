namespace DalEntities
{
    public class Configuration
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int NumberValue { get; set; }

        public Event Event { get; set; } = null!;
    }
}
