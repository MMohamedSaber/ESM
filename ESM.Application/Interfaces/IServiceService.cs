using ESM.Application.DTOs;

namespace ESM.Application.Interfaces;

public interface IServiceService
{
    Task<IEnumerable<ServiceDto>> GetAllAsync();
    Task<ServiceDto?> GetByIdAsync(Guid id);
    Task<ServiceDto> CreateAsync(CreateServiceDto dto);
    Task UpdateAsync(Guid id, CreateServiceDto dto);
    Task DeleteAsync(Guid id);
}
