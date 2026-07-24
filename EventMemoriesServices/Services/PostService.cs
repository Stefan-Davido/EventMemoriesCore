using DalEntities;
using Dal.Repositories;
using EventMemoriesServices.DTOs;
using SharedItems.Services;

namespace EventMemoriesServices.Services
{
    public interface IPostService
    {
        Task<PostDto?> GetPostByIdAsync(Guid id);
        Task<IEnumerable<PostDto>> GetAllPostsAsync();
        Task<IEnumerable<PostDto>> GetPostsByEventAsync(Guid eventId);
        Task<IEnumerable<PostDto>> GetPostsByUserAsync(Guid userId);
        Task<PostDto> CreatePostAsync(CreatePostDto dto, Guid userId);
        Task<PostDto?> UpdatePostAsync(Guid id, UpdatePostDto dto);
        Task<bool> DeletePostAsync(Guid id);
    }

    public class PostService : IPostService
    {
        private readonly IPostRepository _repository;
        private readonly IBlobStorageService _blobStorageService;

        public PostService(IPostRepository repository, IBlobStorageService blobStorageService)
        {
            _repository = repository;
            _blobStorageService = blobStorageService;
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid id)
        {
            var post = await _repository.GetByIdAsync(id);
            return post != null ? MapToDto(post) : null;
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _repository.GetAllAsync();
            return posts.Select(MapToDto);
        }

        public async Task<IEnumerable<PostDto>> GetPostsByEventAsync(Guid eventId)
        {
            var posts = await _repository.GetPostsByEventAsync(eventId);
            return posts.Select(MapToDto);
        }

        public async Task<IEnumerable<PostDto>> GetPostsByUserAsync(Guid userId)
        {
            var posts = await _repository.GetPostsByUserAsync(userId);
            return posts.Select(MapToDto);
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, Guid userId)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            var mediaUrls = new List<string>();

            if (string.IsNullOrEmpty(createPostDto.EventId.ToString()))
                throw new Exception($"Missing paramer EventId!");

            foreach (var file in createPostDto.Files)
            {
                if (!allowedTypes.Contains(file.ContentType))
                    throw new Exception($"Unsupported file type: {file}");

                if (file.Length == 0)
                    continue;

                var url = await _blobStorageService.UploadAsync(file);
                mediaUrls.Add(url);
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventId = createPostDto.EventId,
                MediaUrls = mediaUrls,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            await _repository.AddAsync(post);
            await _repository.SaveChangesAsync();
            return MapToDto(post);
        }

        public async Task<PostDto?> UpdatePostAsync(Guid id, UpdatePostDto dto)
        {
            var post = await _repository.GetByIdAsync(id);
            if (post == null)
                return null;

            if (dto.MediaUrls != null)
            {
                post.MediaUrls = dto.MediaUrls;
                post.Updated = DateTime.Now;
            }

            await _repository.UpdateAsync(post);
            await _repository.SaveChangesAsync();
            return MapToDto(post);
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var result = await _repository.SoftDeleteAsync(id);

            if (result)
                await _repository.SaveChangesAsync();
            return result;
        }

        private static PostDto MapToDto(Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                EventId = post.EventId,
                MediaUrls = post.MediaUrls,
                //Caption = post.C
            };
        }
    }
}
