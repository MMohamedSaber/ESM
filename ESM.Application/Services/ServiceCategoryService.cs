using ESM.Application.DTOs;
using ESM.Application.Interfaces;
using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Services;

public class ServiceCategoryService : IServiceCategoryService
{
    private readonly IEsmDbContext _context;

    public ServiceCategoryService(IEsmDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceCategoryDto> CreateAsync(CreateServiceCategoryDto dto)
    {
        var category = new ServiceCategory
        {
            Name = dto.Name
        };

        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        return new ServiceCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _context.ServiceCategories.FindAsync(id);
        if (category != null)
        {
            _context.ServiceCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ServiceCategoryDto>> GetAllAsync()
    {
        return await _context.ServiceCategories
            .Select(c => new ServiceCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<ServiceCategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _context.ServiceCategories.FindAsync(id);
        if (category == null) return null;

        return new ServiceCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task UpdateAsync(Guid id, CreateServiceCategoryDto dto)
    {
        var category = await _context.ServiceCategories.FindAsync(id);
        if (category != null)
        {
            category.Name = dto.Name;
            await _context.SaveChangesAsync();
        }
    }
}
