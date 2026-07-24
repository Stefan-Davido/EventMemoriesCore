using Microsoft.AspNetCore.Http;

namespace EventMemoriesServices.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public string Caption { get; set; }
    }

    public class CreatePostDto
    {
        public Guid EventId { get; set; }
        public string Caption { get; set; }
        public List<IFormFile> Files { get; set; } = new();
    }

    public class UpdatePostDto
    {
        public List<string>? MediaUrls { get; set; }
    }
}
