using ESM.Application.Common.Models;
using ESM.Application.DTOs.Department;
using ESM.Application.Interfaces;
using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IEsmDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DepartmentService(IEsmDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
    {
        var orgId = _currentUserService.OrganizationId;
        if (orgId == null)
            return ApiResponse<DepartmentDto>.ErrorResponse("Unauthorized", "Unable to determine your organization.");

        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description,
            OrganizationId = orgId.Value
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return ApiResponse<DepartmentDto>.SuccessResponse(MapToDto(department));
    }

    public async Task<ApiResponse<IEnumerable<DepartmentDto>>> GetAllAsync()
    {
        var query = _context.Departments.AsQueryable();

        // Apply tenant isolation
        if (!_currentUserService.IsSuperAdmin)
        {
            var orgId = _currentUserService.OrganizationId;
            query = query.Where(d => d.OrganizationId == orgId);
        }

        var departments = await query
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                OrganizationId = d.OrganizationId
            })
            .ToListAsync();

        return ApiResponse<IEnumerable<DepartmentDto>>.SuccessResponse(departments);
    }

    private static DepartmentDto MapToDto(Department department) => new()
    {
        Id = department.Id,
        Name = department.Name,
        Description = department.Description,
        OrganizationId = department.OrganizationId
    };
}
