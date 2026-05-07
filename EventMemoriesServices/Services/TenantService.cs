using DalEntities;
using Dal.Repositories;
using EventMemoriesServices.DTOs;

namespace EventMemoriesServices.Services
{
    public interface ITenantService
    {
        Task<TenantDto?> GetTenantByIdAsync(Guid id);
        Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
        Task<IEnumerable<TenantDto>> GetTenantsByOwnerAsync(Guid ownerId);
        Task<TenantDto> CreateTenantAsync(CreateTenantDto dto, Guid ownerId);
        Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantDto dto);
        Task<bool> DeleteTenantAsync(Guid id);
    }

    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;

        public TenantService(ITenantRepository repository)
        {
            _repository = repository;
        }

        public async Task<TenantDto?> GetTenantByIdAsync(Guid id)
        {
            var tenant = await _repository.GetByIdAsync(id);
            return tenant != null ? MapToDto(tenant) : null;
        }

        public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
        {
            var tenants = await _repository.GetAllAsync();
            return tenants.Select(MapToDto);
        }

        public async Task<IEnumerable<TenantDto>> GetTenantsByOwnerAsync(Guid ownerId)
        {
            var tenants = await _repository.GetTenantsByOwnerAsync(ownerId);
            return tenants.Select(MapToDto);
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto, Guid ownerId)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Created = DateTime.UtcNow,
                OwnerId = ownerId
            };

            await _repository.AddAsync(tenant);
            await _repository.SaveChangesAsync();
            return MapToDto(tenant);
        }

        public async Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantDto dto)
        {
            var tenant = await _repository.GetByIdAsync(id);
            if (tenant == null)
                return null;

            if (!string.IsNullOrEmpty(dto.Name))
                tenant.Name = dto.Name;

            if (dto.Description != null)
                tenant.Description = dto.Description;

            await _repository.UpdateAsync(tenant);
            await _repository.SaveChangesAsync();
            return MapToDto(tenant);
        }

        public async Task<bool> DeleteTenantAsync(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result)
                await _repository.SaveChangesAsync();
            return result;
        }

        private static TenantDto MapToDto(Tenant tenant)
        {
            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Description = tenant.Description,
                Created = tenant.Created,
                OwnerId = tenant.OwnerId
            };
        }
    }
}
