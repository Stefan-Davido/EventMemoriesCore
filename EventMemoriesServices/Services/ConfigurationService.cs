using DalEntities;
using Dal.Repositories;
using EventMemoriesServices.DTOs;

namespace EventMemoriesServices.Services
{
    public interface IConfigurationService
    {
        Task<ConfigurationDto?> GetConfigurationByIdAsync(Guid id);
        Task<IEnumerable<ConfigurationDto>> GetAllConfigurationsAsync();
        Task<IEnumerable<ConfigurationDto>> GetConfigurationsByEventAsync(Guid eventId);
        Task<ConfigurationDto?> GetConfigurationByNameAsync(Guid eventId, string name);
        Task<ConfigurationDto> CreateConfigurationAsync(CreateConfigurationDto dto);
        Task<ConfigurationDto?> UpdateConfigurationAsync(Guid id, UpdateConfigurationDto dto);
        Task<bool> DeleteConfigurationAsync(Guid id);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _repository;

        public ConfigurationService(IConfigurationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConfigurationDto?> GetConfigurationByIdAsync(Guid id)
        {
            var config = await _repository.GetByIdAsync(id);
            return config != null ? MapToDto(config) : null;
        }

        public async Task<IEnumerable<ConfigurationDto>> GetAllConfigurationsAsync()
        {
            var configs = await _repository.GetAllAsync();
            return configs.Select(MapToDto);
        }

        public async Task<IEnumerable<ConfigurationDto>> GetConfigurationsByEventAsync(Guid eventId)
        {
            var configs = await _repository.GetConfigurationsByEventAsync(eventId);
            return configs.Select(MapToDto);
        }

        public async Task<ConfigurationDto?> GetConfigurationByNameAsync(Guid eventId, string name)
        {
            var config = await _repository.GetConfigurationByNameAsync(eventId, name);
            return config != null ? MapToDto(config) : null;
        }

        public async Task<ConfigurationDto> CreateConfigurationAsync(CreateConfigurationDto dto)
        {
            var config = new Configuration
            {
                Id = Guid.NewGuid(),
                EventId = dto.EventId,
                Name = dto.Name,
                Value = dto.Value,
                NumberValue = dto.NumberValue
            };

            await _repository.AddAsync(config);
            await _repository.SaveChangesAsync();
            return MapToDto(config);
        }

        public async Task<ConfigurationDto?> UpdateConfigurationAsync(Guid id, UpdateConfigurationDto dto)
        {
            var config = await _repository.GetByIdAsync(id);
            if (config == null)
                return null;

            if (!string.IsNullOrEmpty(dto.Name))
                config.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Value))
                config.Value = dto.Value;

            if (dto.NumberValue.HasValue)
                config.NumberValue = dto.NumberValue.Value;

            await _repository.UpdateAsync(config);
            await _repository.SaveChangesAsync();
            return MapToDto(config);
        }

        public async Task<bool> DeleteConfigurationAsync(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result)
                await _repository.SaveChangesAsync();
            return result;
        }

        private static ConfigurationDto MapToDto(Configuration config)
        {
            return new ConfigurationDto
            {
                Id = config.Id,
                EventId = config.EventId,
                Name = config.Name,
                Value = config.Value,
                NumberValue = config.NumberValue
            };
        }
    }
}
