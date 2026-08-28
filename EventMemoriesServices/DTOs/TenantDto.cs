namespace EventMemoriesServices.DTOs
{
    public class TenantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class CreateTenantDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateTenantDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
