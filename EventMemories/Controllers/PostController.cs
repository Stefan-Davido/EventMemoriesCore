using EventMemoriesServices.DTOs;
using EventMemoriesServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedItems;

namespace EventMemories.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;

        public PostController(IPostService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPostById(Guid id)
        {
            var post = await _service.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts()
        {
            var posts = await _service.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByEvent(Guid eventId)
        {
            var posts = await _service.GetPostsByEventAsync(eventId);
            return Ok(posts);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByUser(Guid userId)
        {
            var posts = await _service.GetPostsByUserAsync(userId);
            return Ok(posts);
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            var userIdString = User.FindFirst(InternalClaimTypes.UserId)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Unable to identify user.");
            }

            var post = await _service.CreatePostAsync(createPostDto, userId);
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PostDto>> UpdatePost(Guid id, [FromBody] UpdatePostDto dto)
        {
            var post = await _service.UpdatePostAsync(id, dto);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _service.DeletePostAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
