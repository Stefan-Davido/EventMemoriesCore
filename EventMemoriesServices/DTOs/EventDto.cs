using DalEntities;

namespace EventMemoriesServices.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public Guid OwnerId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? EventDateEnd { get; set; }
        public string? Description { get; set; }
        public TenantSubscription Subscription { get; set; }
    }

    public class CreateEventDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? EventDateEnd { get; set; }
        public string? Description { get; set; }
        public TenantSubscription Subscription { get; set; } = TenantSubscription.S;
    }

    public class UpdateEventDto
    {
        public string? Name { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EventDateEnd { get; set; }
        public string? Description { get; set; }
        public TenantSubscription? Subscription { get; set; }
    }
}
