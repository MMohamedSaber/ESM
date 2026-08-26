using ESM.Application.DTOs;
using ESM.Application.Interfaces;
using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IEsmDbContext _context;

    public ServiceService(IEsmDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
    {
        var service = new Service
        {
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            DurationInMinutes = dto.DurationInMinutes,
            IsActive = true
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(service.Id) ?? throw new Exception("Failed to retrieve created service");
    }

    public async Task DeleteAsync(Guid id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service != null)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync()
    {
        return await _context.Services
            .Include(s => s.Category)
            .Select(s => new ServiceDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                CategoryName = s.Category.Name,
                Name = s.Name,
                Description = s.Description,
                BasePrice = s.BasePrice,
                DurationInMinutes = s.DurationInMinutes,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public async Task<ServiceDto?> GetByIdAsync(Guid id)
    {
        var service = await _context.Services
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null) return null;

        return new ServiceDto
        {
            Id = service.Id,
            CategoryId = service.CategoryId,
            CategoryName = service.Category.Name,
            Name = service.Name,
            Description = service.Description,
            BasePrice = service.BasePrice,
            DurationInMinutes = service.DurationInMinutes,
            IsActive = service.IsActive
        };
    }

    public async Task UpdateAsync(Guid id, CreateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id);
        if (service != null)
        {
            service.CategoryId = dto.CategoryId;
            service.Name = dto.Name;
            service.Description = dto.Description;
            service.BasePrice = dto.BasePrice;
            service.DurationInMinutes = dto.DurationInMinutes;
            
            await _context.SaveChangesAsync();
        }
    }
}
