using EventMemoriesServices.DTOs;
using EventMemoriesServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DalEntities;

namespace EventMemories.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InfoController : ControllerBase
    {
        private readonly IInfoService _service;

        public InfoController(IInfoService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InfoDto>> GetInfoById(int id)
        {
            var info = await _service.GetInfoByIdAsync(id);
            if (info == null)
                return NotFound();
            return Ok(info);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InfoDto>>> GetAllInfos()
        {
            var infos = await _service.GetAllInfosAsync();
            return Ok(infos);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<InfoDto>>> GetInfosByEvent(Guid eventId)
        {
            var infos = await _service.GetInfosByEventAsync(eventId);
            return Ok(infos);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<InfoDto>>> GetInfosByUser(Guid userId)
        {
            var infos = await _service.GetInfosByUserAsync(userId);
            return Ok(infos);
        }

        [HttpGet("level/{level}")]
        public async Task<ActionResult<IEnumerable<InfoDto>>> GetInfosByLevel(int level)
        {
            if (!Enum.IsDefined(typeof(InfoLevel), level))
                return BadRequest("Invalid info level.");

            var infos = await _service.GetInfosByLevelAsync((InfoLevel)level);
            return Ok(infos);
        }

        [HttpPost]
        public async Task<ActionResult<InfoDto>> CreateInfo([FromBody] CreateInfoDto dto)
        {
            var userIdString = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Unable to identify user.");

            var info = await _service.CreateInfoAsync(dto, userId);
            return CreatedAtAction(nameof(GetInfoById), new { id = info.Id }, info);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InfoDto>> UpdateInfo(int id, [FromBody] UpdateInfoDto dto)
        {
            var info = await _service.UpdateInfoAsync(id, dto);
            if (info == null)
                return NotFound();
            return Ok(info);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInfo(int id)
        {
            var result = await _service.DeleteInfoAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
