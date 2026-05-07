namespace EventMemoriesServices.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
    }

    public class CreatePostDto
    {
        public Guid EventId { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
    }

    public class UpdatePostDto
    {
        public List<string>? MediaUrls { get; set; }
    }
}
