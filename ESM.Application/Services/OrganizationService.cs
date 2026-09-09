using ESM.Application.Common.Models;
using ESM.Application.DTOs.Organization;
using ESM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IEsmDbContext _context;

    public OrganizationService(IEsmDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<IEnumerable<OrganizationDto>>> GetAllAsync()
    {
        var organizations = await _context.Organizations
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                NameEn = o.NameEn,
                NameAr = o.NameAr
            })
            .ToListAsync();

        return ApiResponse<IEnumerable<OrganizationDto>>.SuccessResponse(organizations);
    }
}
