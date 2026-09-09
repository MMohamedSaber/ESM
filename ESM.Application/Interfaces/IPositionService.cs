using ESM.Application.Common.Models;
using ESM.Application.DTOs.Position;

namespace ESM.Application.Interfaces;

public interface IPositionService
{
    Task<ApiResponse<IEnumerable<PositionDto>>> GetAllAsync();
}
