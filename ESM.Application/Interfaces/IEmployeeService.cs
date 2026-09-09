using ESM.Application.Common.Models;
using ESM.Application.DTOs.Employee;

namespace ESM.Application.Interfaces;

public interface IEmployeeService
{
    Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto dto);
    Task<ApiResponse<EmployeeDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllAsync();
    Task<ApiResponse<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
