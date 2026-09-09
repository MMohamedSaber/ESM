using ESM.Application.Common.Models;
using ESM.Application.DTOs.Position;
using ESM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESM.Application.Services;

public class PositionService : IPositionService
{
    private readonly IEsmDbContext _context;

    public PositionService(IEsmDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<IEnumerable<PositionDto>>> GetAllAsync()
    {
        var positions = await _context.Positions
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync();

        return ApiResponse<IEnumerable<PositionDto>>.SuccessResponse(positions);
    }
}
