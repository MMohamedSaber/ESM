using ESM.Application.Common.Models;
using ESM.Application.DTOs.Department;

namespace ESM.Application.Interfaces;

public interface IDepartmentService
{
    Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
    Task<ApiResponse<IEnumerable<DepartmentDto>>> GetAllAsync();
}
