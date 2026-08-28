using DalEntities;
using Dal.Repositories;
using EventMemoriesServices.DTOs;

namespace EventMemoriesServices.Services
{
    public interface IInfoService
    {
        Task<InfoDto?> GetInfoByIdAsync(int id);
        Task<IEnumerable<InfoDto>> GetAllInfosAsync();
        Task<IEnumerable<InfoDto>> GetInfosByEventAsync(Guid eventId);
        Task<IEnumerable<InfoDto>> GetInfosByUserAsync(Guid userId);
        Task<IEnumerable<InfoDto>> GetInfosByLevelAsync(InfoLevel level);
        Task<InfoDto> CreateInfoAsync(CreateInfoDto dto, Guid userId);
        Task<InfoDto?> UpdateInfoAsync(int id, UpdateInfoDto dto);
        Task<bool> DeleteInfoAsync(int id);
    }

    public class InfoService : IInfoService
    {
        private readonly IInfoRepository _repository;

        public InfoService(IInfoRepository repository)
        {
            _repository = repository;
        }

        public async Task<InfoDto?> GetInfoByIdAsync(int id)
        {
            var info = await _repository.GetByIdAsync(id);
            return info != null ? MapToDto(info) : null;
        }

        public async Task<IEnumerable<InfoDto>> GetAllInfosAsync()
        {
            var infos = await _repository.GetAllAsync();
            return infos.Select(MapToDto);
        }

        public async Task<IEnumerable<InfoDto>> GetInfosByEventAsync(Guid eventId)
        {
            var infos = await _repository.GetInfosByEventAsync(eventId);
            return infos.Select(MapToDto);
        }

        public async Task<IEnumerable<InfoDto>> GetInfosByUserAsync(Guid userId)
        {
            var infos = await _repository.GetInfosByUserAsync(userId);
            return infos.Select(MapToDto);
        }

        public async Task<IEnumerable<InfoDto>> GetInfosByLevelAsync(InfoLevel level)
        {
            var infos = await _repository.GetInfosByLevelAsync(level);
            return infos.Select(MapToDto);
        }

        public async Task<InfoDto> CreateInfoAsync(CreateInfoDto dto, Guid userId)
        {
            var info = new Info
            {
                Level = dto.Level,
                Text = dto.Text,
                Date = dto.Date,
                EventId = dto.EventId,
                UserId = userId
            };

            await _repository.AddAsync(info);
            await _repository.SaveChangesAsync();
            return MapToDto(info);
        }

        public async Task<InfoDto?> UpdateInfoAsync(int id, UpdateInfoDto dto)
        {
            var info = await _repository.GetByIdAsync(id);
            if (info == null)
                return null;

            if (dto.Level.HasValue)
                info.Level = dto.Level.Value;

            if (!string.IsNullOrEmpty(dto.Text))
                info.Text = dto.Text;

            if (dto.Date.HasValue)
                info.Date = dto.Date;

            await _repository.UpdateAsync(info);
            await _repository.SaveChangesAsync();
            return MapToDto(info);
        }

        public async Task<bool> DeleteInfoAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result)
                await _repository.SaveChangesAsync();
            return result;
        }

        private static InfoDto MapToDto(Info info)
        {
            return new InfoDto
            {
                Id = info.Id,
                Level = info.Level,
                Text = info.Text,
                Date = info.Date,
                EventId = info.EventId,
                UserId = info.UserId
            };
        }
    }
}
