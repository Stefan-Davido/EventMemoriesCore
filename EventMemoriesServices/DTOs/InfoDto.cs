using DalEntities;

namespace EventMemoriesServices.DTOs
{
    public class InfoDto
    {
        public int Id { get; set; }
        public InfoLevel Level { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateInfoDto
    {
        public InfoLevel Level { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public Guid EventId { get; set; }
    }

    public class UpdateInfoDto
    {
        public InfoLevel? Level { get; set; }
        public string? Text { get; set; }
        public DateTime? Date { get; set; }
    }
}
