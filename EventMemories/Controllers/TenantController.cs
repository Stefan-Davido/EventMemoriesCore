using EventMemoriesServices.DTOs;
using EventMemoriesServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedItems;

namespace EventMemories.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _service;

        public TenantController(ITenantService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetTenantById(int id)
        {
            var tenant = await _service.GetTenantByIdAsync(id);
            if (tenant == null)
                return NotFound();
            return Ok(tenant);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAllTenants()
        {
            var tenants = await _service.GetAllTenantsAsync();
            return Ok(tenants);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetTenantsByOwner(Guid ownerId)
        {
            var tenants = await _service.GetTenantsByOwnerAsync(ownerId);
            return Ok(tenants);
        }

        [HttpPost]
        public async Task<ActionResult<TenantDto>> CreateTenant([FromBody] CreateTenantDto dto)
        {
            var userIdString = User.FindFirst(InternalClaimTypes.UserId)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Unable to identify user.");
            }

            var tenant = await _service.CreateTenantAsync(dto, userId);
            return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, tenant);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TenantDto>> UpdateTenant(int id, [FromBody] UpdateTenantDto dto)
        {
            var tenant = await _service.UpdateTenantAsync(id, dto);
            if (tenant == null)
                return NotFound();
            return Ok(tenant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var result = await _service.DeleteTenantAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
