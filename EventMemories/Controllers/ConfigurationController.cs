using EventMemoriesServices.DTOs;
using EventMemoriesServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMemories.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _service;

        public ConfigurationController(IConfigurationService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConfigurationDto>> GetConfigurationById(Guid id)
        {
            var configuration = await _service.GetConfigurationByIdAsync(id);
            if (configuration == null)
                return NotFound();
            return Ok(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfigurationDto>>> GetAllConfigurations()
        {
            var configurations = await _service.GetAllConfigurationsAsync();
            return Ok(configurations);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<ConfigurationDto>>> GetConfigurationsByEvent(Guid eventId)
        {
            var configurations = await _service.GetConfigurationsByEventAsync(eventId);
            return Ok(configurations);
        }

        [HttpGet("event/{eventId}/name/{name}")]
        public async Task<ActionResult<ConfigurationDto>> GetConfigurationByName(Guid eventId, string name)
        {
            var configuration = await _service.GetConfigurationByNameAsync(eventId, name);
            if (configuration == null)
                return NotFound();
            return Ok(configuration);
        }

        [HttpPost]
        public async Task<ActionResult<ConfigurationDto>> CreateConfiguration([FromBody] CreateConfigurationDto dto)
        {
            var configuration = await _service.CreateConfigurationAsync(dto);
            return CreatedAtAction(nameof(GetConfigurationById), new { id = configuration.Id }, configuration);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ConfigurationDto>> UpdateConfiguration(Guid id, [FromBody] UpdateConfigurationDto dto)
        {
            var configuration = await _service.UpdateConfigurationAsync(id, dto);
            if (configuration == null)
                return NotFound();
            return Ok(configuration);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguration(Guid id)
        {
            var result = await _service.DeleteConfigurationAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
