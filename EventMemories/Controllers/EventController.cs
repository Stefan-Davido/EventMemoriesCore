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
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEventById(Guid id)
        {
            var eventEntity = await _service.GetEventByIdAsync(id);
            if (eventEntity == null)
                return NotFound();
            return Ok(eventEntity);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
        {
            var events = await _service.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByTenant(Guid tenantId)
        {
            var events = await _service.GetEventsByTenantAsync(tenantId);
            return Ok(events);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByOwner(Guid ownerId)
        {
            var events = await _service.GetEventsByOwnerAsync(ownerId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto dto)
        {
            var userIdString = User.FindFirst(InternalClaimTypes.UserId)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Unable to identify user.");
            }

            var eventEntity = await _service.CreateEventAsync(dto, userId);
            return CreatedAtAction(nameof(GetEventById), new { id = eventEntity.Id }, eventEntity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EventDto>> UpdateEvent(Guid id, [FromBody] UpdateEventDto dto)
        {
            var eventEntity = await _service.UpdateEventAsync(id, dto);
            if (eventEntity == null)
                return NotFound();
            return Ok(eventEntity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var result = await _service.DeleteEventAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
