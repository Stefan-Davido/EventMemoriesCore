using DalEntities;
using Dal.Repositories;
using EventMemoriesServices.DTOs;

namespace EventMemoriesServices.Services
{
    public interface IEventService
    {
        Task<EventDto?> GetEventByIdAsync(Guid id);
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<IEnumerable<EventDto>> GetEventsByTenantAsync(int tenantId);
        Task<IEnumerable<EventDto>> GetEventsByOwnerAsync(Guid ownerId);
        Task<EventDto> CreateEventAsync(CreateEventDto dto, Guid ownerId);
        Task<EventDto?> UpdateEventAsync(Guid id, UpdateEventDto dto);
        Task<bool> DeleteEventAsync(Guid id);
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<EventDto?> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _repository.GetByIdAsync(id);
            return eventEntity != null ? MapToDto(eventEntity) : null;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _repository.GetAllAsync();
            return events.Select(MapToDto);
        }

        public async Task<IEnumerable<EventDto>> GetEventsByTenantAsync(int tenantId)
        {
            var events = await _repository.GetEventsByTenantAsync(tenantId);
            return events.Select(MapToDto);
        }

        public async Task<IEnumerable<EventDto>> GetEventsByOwnerAsync(Guid ownerId)
        {
            var events = await _repository.GetEventsByOwnerAsync(ownerId);
            return events.Select(MapToDto);
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto dto, Guid ownerId)
        {
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                TenantId = dto.TenantId,
                OwnerId = ownerId,
                CreatedTime = DateTime.UtcNow,
                EventDate = dto.EventDate,
                EventDateEnd = dto.EventDateEnd,
                Description = dto.Description,
                Subscription = dto.Subscription
            };

            await _repository.AddAsync(eventEntity);
            await _repository.SaveChangesAsync();
            return MapToDto(eventEntity);
        }

        public async Task<EventDto?> UpdateEventAsync(Guid id, UpdateEventDto dto)
        {
            var eventEntity = await _repository.GetByIdAsync(id);
            if (eventEntity == null)
                return null;

            if (!string.IsNullOrEmpty(dto.Name))
                eventEntity.Name = dto.Name;

            if (dto.EventDate.HasValue)
                eventEntity.EventDate = dto.EventDate.Value;

            if (dto.EventDateEnd.HasValue)
                eventEntity.EventDateEnd = dto.EventDateEnd.Value;

            if (dto.Description != null)
                eventEntity.Description = dto.Description;

            if (dto.Subscription.HasValue)
                eventEntity.Subscription = dto.Subscription.Value;

            await _repository.UpdateAsync(eventEntity);
            await _repository.SaveChangesAsync();
            return MapToDto(eventEntity);
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result)
                await _repository.SaveChangesAsync();
            return result;
        }

        private static EventDto MapToDto(Event eventEntity)
        {
            return new EventDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                CreatedTime = eventEntity.CreatedTime,
                OwnerId = eventEntity.OwnerId,
                TenantId = eventEntity.TenantId,
                EventDate = eventEntity.EventDate,
                EventDateEnd = eventEntity.EventDateEnd,
                Description = eventEntity.Description,
                Subscription = eventEntity.Subscription
            };
        }
    }
}
