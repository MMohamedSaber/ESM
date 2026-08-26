using ESM.Application.DTOs;

namespace ESM.Application.Interfaces;

public interface IServiceCategoryService
{
    Task<IEnumerable<ServiceCategoryDto>> GetAllAsync();
    Task<ServiceCategoryDto?> GetByIdAsync(Guid id);
    Task<ServiceCategoryDto> CreateAsync(CreateServiceCategoryDto dto);
    Task UpdateAsync(Guid id, CreateServiceCategoryDto dto);
    Task DeleteAsync(Guid id);
}
